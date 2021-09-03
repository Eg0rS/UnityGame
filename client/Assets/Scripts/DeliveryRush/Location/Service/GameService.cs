using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using DeliveryRush.Core;
using DeliveryRush.LevelMap.LevelDialogs;
using DeliveryRush.LevelMap.Levels.Descriptor;
using DeliveryRush.LevelMap.Levels.Service;
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

namespace DeliveryRush.Location.Service
{
    public struct DronStats
    {
        public float _durability;
        public float _energy;
        public int _countChips;
        public float _boostSpeedValue;
        public float _boostShieldTime;
        public float _boostSpeedTime;
        public bool _onActiveShield;
        public float _energyFall;
        public float _energyForSpeed;
        public float _maxDurability;
    }

    public enum FailedReasons
    {
        Crashed,
        EnergyFalled,
    }

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
        private GestureService _gestureService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DronService _dronService;

        [Inject]
        private LocationService _locationService;
        
        private LevelDescriptor _levelDescriptor;
        private DronStats _dronStats;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private Coroutine _fallingEnergy;

        private bool IsPlay
        {
            set { _isPlay = value; }
        }

        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _dronId = dronId;
            _levelDescriptor = levelDescriptor;
            _locationService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _gestureService.AddListener<WorldEvent>(WorldEvent.SWIPE, OnSwipe);
            _locationService.SwitchLocation(levelDescriptor);
            _overlayManager.Require().HideLoadingOverlay(true);
            SetStartOptionsDron();
        }

        private void SetStartOptionsDron()
        {
            DronDescriptor dronDescriptor = _dronService.GetDronById(_dronId).DronDescriptor;
            _dronStats._durability = dronDescriptor.Durability;
            _dronStats._maxDurability = dronDescriptor.Durability;
            _dronStats._energy = dronDescriptor.Energy;
            _dronStats._countChips = 0;
            _dronStats._energyFall = 0.05f;
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.WORLD_CREATED, _dronStats));
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ACTIVATE_BOOST, OnActivateBoost);
            CreateDrone(_dronId);
        }

        private void OnSwipe(WorldEvent worldEvent)
        {
            if (_isPlay) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.START_FLIGHT));
            _isPlay = true;
            _fallingEnergy = StartCoroutine(FallEnergy());
            _startTime = Time.time;
        }

        private void OnDronCollision(WorldEvent worldEvent)
        {
            GameObject collisionObject = worldEvent.CollisionObject;
            switch (collisionObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(collisionObject.GetComponent<ObstacleModel>());
                    break;
                case WorldObjectType.Battery:
                    OnTakeBattery(collisionObject.GetComponent<BatteryModel>());
                    break;
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(collisionObject.GetComponent<BonusChipsModel>());
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    OnTakeSpeed(collisionObject.GetComponent<SpeedBoosterModel>());
                    break;
                case WorldObjectType.SHIELD_BUSTER:
                    OnTakeShield(collisionObject.GetComponent<ShieldBoosterModel>());
                    break;
                case WorldObjectType.FINISH:
                    Victory(collisionObject.GetComponent<FinishModel>());
                    break;
            }
        }

        private void OnTakeBattery(BatteryModel component)
        {
            _dronStats._energy += component.Energy;
            component.gameObject.SetActive(false);
        }

        private void OnTakeShield(ShieldBoosterModel component)
        {
            _dronStats._boostShieldTime = component.Duration;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeSpeed(SpeedBoosterModel component)
        {
            _dronStats._boostSpeedTime = component.Duration;
            _dronStats._boostSpeedValue = component.SpeedBoost;
            _dronStats._energyForSpeed = component.NeedsEnergy;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeChip(BonusChipsModel component)
        {
            _dronStats._countChips++;
            component.gameObject.SetActive(false);
            UiUpdate();
        }

        private void OnDronCrash(ObstacleModel component)
        {
            if (_dronStats._onActiveShield) {
                return;
            }
            _dronStats._durability -= component.Damage;
            if (_dronStats._durability <= 0) {
                _dronStats._durability = 0;
                DronFailed(FailedReasons.Crashed);
            }
            UiUpdate();
        }

        private void OnActivateBoost(WorldEvent @event)
        {
            switch (@event.TypeBoost) {
                case WorldObjectType.SHIELD_BUSTER:
                    _dronStats._onActiveShield = true;
                    Invoke(nameof(DisableShield), _dronStats._boostShieldTime);
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    _dronStats._energy -= _dronStats._energyForSpeed;
                    Invoke("DisableSpeed", _dronStats._boostSpeedTime);
                    _gameWorld.Require()
                              .Dispatch(new WorldEvent(WorldEvent.DRON_BOOST_SPEED, _dronStats._boostSpeedValue, _dronStats._boostSpeedTime));
                    break;
            }
        }

        private void DisableShield()
        {
            _dronStats._onActiveShield = false;
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _dronStats));
        }

        public void EndGame()
        {
            if (_fallingEnergy != null) {
                StopCoroutine(_fallingEnergy);
            }
            IsPlay = false;
            Time.timeScale = 0f;
            _locationService.RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _gestureService.RemoveListener<WorldEvent>(WorldEvent.SWIPE, OnSwipe);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.CurrentLevelId, CalculateStars(timeInGame), _dronStats._countChips, timeInGame,
                                               (int) ((_dronStats._durability / _dronStats._maxDurability) * 100));
            }
        }

        private void Victory(FinishModel component)
        {
            SetStatsInProgress(true);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
            _locationService.RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void DronFailed(FailedReasons failReason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(failReason);
        }

        private void CreateDrone(string dronId)
        {
            GameObject parent = GameObject.Find("DronCube");
            Instantiate(Resources.Load<GameObject>(_dronService.GetDronById(dronId).DronDescriptor.Prefab), parent.transform);
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

        private IEnumerator FallEnergy()
        {
            while (_isPlay) {
                _dronStats._energy -= _dronStats._energyFall;
                if (_dronStats._energy <= 0) {
                    _dronStats._energy = 0;
                    UiUpdate();
                    DronFailed(FailedReasons.EnergyFalled);
                } else {
                    UiUpdate();
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}