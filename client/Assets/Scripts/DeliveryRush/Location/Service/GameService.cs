using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Dialog.Service;
using DeliveryRush.Core;
using DeliveryRush.Core.Audio;
using DeliveryRush.Core.Audio.Model;
using DeliveryRush.Core.Audio.Service;
using DeliveryRush.Resource.LevelDialogs;
using DeliveryRush.Resource.Descriptor;
using DeliveryRush.Resource.Service;
using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.BaseModel;
using DeliveryRush.Location.Model.Battery;
using DeliveryRush.Location.Model.BonusChips;
using DeliveryRush.Location.Model.Finish;
using DeliveryRush.Location.Model.Obstacle;
using DeliveryRush.Location.Model.ShieldBooster;
using DeliveryRush.Location.Model.SpeedBooster;
using DeliveryRush.Location.World.Dron.Descriptor;
using DeliveryRush.Location.World.Dron.Service;
using DeliveryRush.World;
using DeliveryRush.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

public struct DronStats
{
    public float _durability;
    public float _energy;
    public int _countChips;
    public bool _onHasShield;
    public bool _onHasSpeed;
    public float _boostSpeedValue;

    public float _boostShieldTime;
    public float _boostSpeedTime;
    public bool _onActiveShield;
    public float _energyFall;
    public float _energyForSpeed;
    public float _MaxDurability;
}

namespace DeliveryRush.Location.Service
{
    [Injectable]
    public class GameService : GameEventDispatcher
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private IGestureService _gestureService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DronService _dronService;

        [Inject]
        private SoundService _soundService;

        private GameObject _levelContainer;

        private DronStats _dronStats;

        private bool _isPlay = false;

        private LevelDescriptor _levelDescriptor;

        private float _startTime = 0;

        private int _speedShift;

        public bool IsPlay
        {
            get { return _isPlay; }
            set { _isPlay = value; }
        }
        public int SpeedShift
        {
            set { _speedShift = value; }

            get { return _speedShift; }
        }

        private void PlaySound(Sound sound)
        {
            _soundService.StopAllSounds();
            _soundService.PlaySound(sound);
        }

        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _levelDescriptor = levelDescriptor;
            DronDescriptor dronDescriptor = _dronService.GetDronById(dronId).DronDescriptor;
            _overlayManager.Require().HideLoadingOverlay(true);
            _levelContainer = GameObject.Find($"Overlay");

            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.ON_COLLISION, DronCollision);
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.ACTIVATE_BOOST, ActivateBoost);
            SpeedShift = _dronService.GetDronById(dronId).DronDescriptor.Mobility;
            _dronStats._durability = dronDescriptor.Durability;
            _dronStats._MaxDurability = dronDescriptor.Durability;
            _dronStats._energy = dronDescriptor.Energy;
            _dronStats._countChips = 0;
            _dronStats._energyFall = 0.15f;
            _gestureService.AddTapHandler(OnTap, false);
        }

        private void OnTap(Tap tap)
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.START_GAME, gameObject));
            _isPlay = true;
            StartCoroutine(FallEnergy());
            _startTime = Time.time;
            _gestureService.RemoveTapHandler(OnTap);
        }

        private void DronCollision(WorldObjectEvent worldObjectEvent)
        {
            GameObject collisionObject = worldObjectEvent._collisionObject;
            switch (collisionObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(collisionObject.GetComponent<ObstacleModel>());
                    PlaySound(GameSounds.COLLISION);
                    break;
                case WorldObjectType.Battery:
                    OnTakeBattery(collisionObject.GetComponent<BatteryModel>());
                    PlaySound(GameSounds.BOOSTER_PICKUP);
                    break;
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(collisionObject.GetComponent<BonusChipsModel>());
                    PlaySound(GameSounds.CHIP_PICKUP);
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    OnTakeSpeed(collisionObject.GetComponent<SpeedBoosterModel>());
                    PlaySound(GameSounds.BOOSTER_PICKUP);
                    break;
                case WorldObjectType.SHIELD_BUSTER:
                    OnTakeShield(collisionObject.GetComponent<ShieldBoosterModel>());
                    PlaySound(GameSounds.BOOSTER_PICKUP);
                    break;
                case WorldObjectType.FINISH:
                    Victory(collisionObject.GetComponent<FinishModel>());
                    PlaySound(GameSounds.SHOW_DIALOG);
                    break;
                default:
                    break;
            }
        }

        private IEnumerator FallEnergy()
        {
            while (_isPlay) {
                _dronStats._energy -= _dronStats._energyFall;
                if (_dronStats._energy <= 0) {
                    _dronStats._energy = 0;
                    UiUpdate();
                    DronFailed(1);
                } else {
                    UiUpdate();
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        private void OnTakeBattery(BatteryModel getComponent)
        {
            _dronStats._energy += getComponent.Energy;
            getComponent.gameObject.SetActive(false);
        }

        private void OnTakeShield(ShieldBoosterModel getComponent)
        {
            _dronStats._onHasShield = true;
            _dronStats._boostShieldTime = getComponent.Duration;
            getComponent.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_BOOST, getComponent.ObjectType));
        }

        private void OnTakeSpeed(SpeedBoosterModel getComponent)
        {
            _dronStats._onHasSpeed = true;
            _dronStats._boostSpeedTime = getComponent.Duration;
            _dronStats._boostSpeedValue = getComponent.SpeedBoost;
            _dronStats._energyForSpeed = getComponent.NeedsEnergy;
            getComponent.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_BOOST, getComponent.ObjectType));
        }

        private void OnTakeChip(BonusChipsModel getComponent)
        {
            _dronStats._countChips++;
            UiUpdate();
            getComponent.gameObject.SetActive(false);
        }

        private void OnDronCrash(ObstacleModel getComponent)
        {
            if (_dronStats._onActiveShield) return;
            _dronStats._durability -= getComponent.Damage;
            if (_dronStats._durability <= 0) {
                _dronStats._durability = 0;
                DronFailed(0);
            }
            UiUpdate();
        }

        private void ActivateBoost(WorldObjectEvent objectEvent)
        {
            switch (objectEvent._typeBoost) {
                case WorldObjectType.SHIELD_BUSTER:
                    _dronStats._onActiveShield = true;
                    Invoke(nameof(DisableShield), _dronStats._boostShieldTime);
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    _dronStats._energy -= _dronStats._energyForSpeed;
                    Invoke("DisableSpeed", _dronStats._boostSpeedTime);
                    _gameWorld.Require()
                              .Dispatch(new WorldObjectEvent(WorldObjectEvent.DRON_BOOST_SPEED, _dronStats._boostSpeedValue,
                                                             _dronStats._boostSpeedTime));
                    break;
                default:
                    break;
            }
        }

        private void DisableShield()
        {
            _dronStats._onActiveShield = false;
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.UI_UPDATE, _dronStats));
        }

        private void EndGame(bool isWin)
        {
            _isPlay = false;
            float timeInGame = Time.time - _startTime;
            Time.timeScale = 0f;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.CurrentLevelId, CalculateStars(timeInGame), _dronStats._countChips, timeInGame,
                                               (int) ((_dronStats._durability / _dronStats._MaxDurability) * 100));
            }
        }

        private void Victory(FinishModel getComponent)
        {
            EndGame(true);
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DronFailed(short reason)
        {
            EndGame(false);
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(reason);
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            if (_dronStats._durability >= _levelDescriptor.NecessaryDurability) {
                countStars++;
            }
            if (_dronStats._countChips >= _levelDescriptor.NecessaryCountChips) {
                countStars++;
            }
            if (timeInGame <= _levelDescriptor.NecessaryTime) {
                countStars++;
            }

            return countStars;
        }
    }
}