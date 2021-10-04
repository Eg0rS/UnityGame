using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Dialog.Service;
using Cinemachine;
using Drone.Core;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.Location.Model.BonusChips;
using Drone.Location.Model.Finish;
using Drone.Location.Model.Obstacle;
using Drone.Location.Model.ShieldBooster;
using Drone.Location.Model.SpeedBooster;
using Drone.Location.World.Dron.Service;
using Drone.Location.World.Dron.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service
{
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
        private IGestureService _gestureService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DronService _dronService;

        [Inject]
        private LocationService _locationService;

        private DroneModel _droneModel;
        
        
        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private Coroutine _fallingEnergy;
        public float _energyForSpeed;
        public float _speedBoost;
        public float _accelerationBoost;
        public float _boostShieldTime;
        public bool _onActiveShield;

        private bool IsPlay
        {
            set { _isPlay = value; }
        }

        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _dronId = dronId;
            _levelDescriptor = levelDescriptor;
            _locationService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _locationService.SwitchLocation(levelDescriptor);
            _overlayManager.Require().HideLoadingOverlay(true);
            SetStartOptionsDron();
        }

        private void SetStartOptionsDron()
        {
            _droneModel = new DroneModel(_dronService.GetDronById(_dronId).DronDescriptor);
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
            _gestureService.AddAnyTouchHandler(OnAnyTouch, false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.WORLD_CREATED, _droneModel));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, _droneModel));
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ACTIVATE_BOOST, OnActivateBoost);
            CreateDrone(_dronId);
        }

        private void OnAnyTouch(AnyTouch anyTouch)
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
            _droneModel.energy += component.Energy;
            component.gameObject.SetActive(false);
        }

        private void OnTakeShield(ShieldBoosterModel component)
        {
            _boostShieldTime = component.Duration;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeSpeed(SpeedBoosterModel component)
        {
            _speedBoost = component.SpeedBoost;
            _accelerationBoost= component.AccelerationBoost;
            _energyForSpeed = component.NeedsEnergy;
            component.gameObject.SetActive(false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_BOOST, component.ObjectType));
        }

        private void OnTakeChip(BonusChipsModel component)
        {
            _droneModel.countChips++;
            component.gameObject.SetActive(false);
            UiUpdate();
        }

        private void OnDronCrash(ObstacleModel component)
        {
            if (_onActiveShield) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASH));
            _droneModel.durability -= component.Damage;
            if (_droneModel.durability <= 0) {
                _droneModel.durability = 0;
                DronFailed(FailedReasons.Crashed);
            }
            UiUpdate();
        }

        private void OnActivateBoost(WorldEvent @event)
        {
            switch (@event.TypeBoost) {
                case WorldObjectType.SHIELD_BUSTER:
                    _onActiveShield = true;
                    Invoke(nameof(DisableShield), _boostShieldTime);
                    break;
                case WorldObjectType.SPEED_BUSTER:
                    _droneModel.energy -= _energyForSpeed;
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRON_BOOST_SPEED, _speedBoost, _accelerationBoost));
                    break;
            }
        }

        private void DisableShield()
        {
            _onActiveShield = false;
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        public void EndGame()
        {
            if (_fallingEnergy != null) {
                StopCoroutine(_fallingEnergy);
            }
            IsPlay = false;
            Time.timeScale = 0f;
            _locationService.RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _gestureService.RemoveAnyTouchHandler(OnAnyTouch);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.CurrentLevelId, CalculateStars(timeInGame), _droneModel.countChips, timeInGame,
                                               (int) ((_droneModel.durability / _droneModel.maxDurability) * 100));
            }
        }

        private void Victory(FinishModel component)
        {
            SetStatsInProgress(true);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DronFailed(FailedReasons failReason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(failReason);
        }

        private void CreateDrone(string dronId)
        {
            GameObject parent = _gameWorld.Require().GetGameObjectByName("DronCube");
            GameObject drone = Instantiate(Resources.Load<GameObject>(_dronService.GetDronById(dronId).DronDescriptor.Prefab));
            _gameWorld.Require().AddGameObject(drone, parent);
            CinemachineVirtualCamera camera = _gameWorld.Require().GetGameObjectByName("CM vcam1")?.GetComponent<CinemachineVirtualCamera>();
            camera.Follow = drone.transform;
            camera.LookAt = drone.transform;
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            if (_droneModel.durability >= _levelDescriptor.NecessaryDurability) {
                countStars++;
            }
            if (_droneModel.countChips >= _levelDescriptor.NecessaryCountChips) {
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
                _droneModel.energy -= _droneModel.energyFall;
                if (_droneModel.energy <= 0) {
                    _droneModel.energy = 0;
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