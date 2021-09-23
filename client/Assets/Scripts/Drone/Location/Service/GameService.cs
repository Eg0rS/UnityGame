using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Core;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.Location.Model.BonusChips;
using Drone.Location.Model.Dron;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.ShieldBooster;
using Drone.Location.Model.SpeedBooster;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service
{
    public struct DronStats
    {
        public float durability;
        public float energy;
        public int countChips;
        public float boostSpeedValue;
        public float boostShieldTime;
        public float boostSpeedTime;
        public bool onActiveShield;
        public float energyFall;
        public float energyForSpeed;
        public float maxDurability;
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
        private DronControlService _dronControlService;

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

        public DronModel GetDroneModelById()
        {
            DronDescriptor dronDescriptor = _dronService.GetDronById(_dronId).DronDescriptor;
            return new DronModel(dronDescriptor.Mobility, dronDescriptor.Durability, dronDescriptor.Energy);
        }

        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _dronId = dronId;
            _levelDescriptor = levelDescriptor;
            _locationService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.END_MOVE, OnSwipe);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.SWIPE, OnSwipe);
            _locationService.SwitchLocation(levelDescriptor);
            _overlayManager.Require().HideLoadingOverlay(true);
            SetStartOptionsDron();
        }

        private void SetStartOptionsDron()
        {
            DronDescriptor dronDescriptor = _dronService.GetDronById(_dronId).DronDescriptor;
         //   _dronStats.durability = dronDescriptor.Durability;
         //   _dronStats.energy = dronDescriptor.Energy;
            _dronStats.durability = 999999;
            _dronStats.energy = 9999999;
            _dronStats.maxDurability = dronDescriptor.Durability; //todo ошибочка
            _dronStats.countChips = 0;
            _dronStats.energyFall = 0.05f;
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
            _dronStats.energy += component.Energy;
            component.gameObject.SetActive(false);
        }

        private void OnTakeShield(ShieldBoosterModel component)
        {
            _dronStats.boostShieldTime = component.Duration;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeSpeed(SpeedBoosterModel component)
        {
            _dronStats.boostSpeedTime = component.Duration;
            _dronStats.boostSpeedValue = component.SpeedBoost;
            _dronStats.energyForSpeed = component.NeedsEnergy;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeChip(BonusChipsModel component)
        {
            _dronStats.countChips++;
            component.gameObject.SetActive(false);
            UiUpdate();
        }

        private void OnDronCrash(ObstacleModel component)
        {
            if (_dronStats.onActiveShield) {
                return;
            }
            _dronStats.durability -= component.Damage;
            if (_dronStats.durability <= 0) {
                _dronStats.durability = 0;
                DronFailed(FailedReasons.Crashed);
            }
            UiUpdate();
        }

        private void OnActivateBoost(WorldEvent @event)
        {
            switch (@event.TypeBoost) {
                case WorldObjectType.SHIELD_BUSTER:
                    _dronStats.onActiveShield = true;
                    Invoke(nameof(DisableShield), _dronStats.boostShieldTime);
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    _dronStats.energy -= _dronStats.energyForSpeed;
                    Invoke("DisableSpeed", _dronStats.boostSpeedTime);
                    _gameWorld.Require()
                              .Dispatch(new WorldEvent(WorldEvent.DRON_BOOST_SPEED, _dronStats.boostSpeedValue, _dronStats.boostSpeedTime));
                    break;
            }
        }

        private void DisableShield()
        {
            _dronStats.onActiveShield = false;
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
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.SWIPE, OnSwipe);
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.END_MOVE, OnSwipe);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.CurrentLevelId, CalculateStars(timeInGame), _dronStats.countChips, timeInGame,
                                               (int) ((_dronStats.durability / _dronStats.maxDurability) * 100));
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
            Instantiate(Resources.Load<GameObject>(_dronService.GetDronById(dronId).DronDescriptor.Prefab), parent.transform);//TODO ошибочка
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            if (_dronStats.durability >= _levelDescriptor.NecessaryDurability) {
                countStars++;
            }
            if (_dronStats.countChips >= _levelDescriptor.NecessaryCountChips) {
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
                _dronStats.energy -= _dronStats.energyFall;
                if (_dronStats.energy <= 0) {
                    _dronStats.energy = 0;
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