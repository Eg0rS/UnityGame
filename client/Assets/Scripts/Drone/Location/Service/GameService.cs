using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Dialog.Service;
using Cinemachine;
using Drone.Core;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
using Drone.Location.Model;
using Drone.Location.Model.Battery;
using Drone.Location.Model.Obstacle;
using Drone.Location.World.Drone.Model;
using Drone.Location.World.Drone.Service;
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
    public class GameService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private IoCProvider<BoosterService> _boosterService;

        [Inject]
        private IGestureService _gestureService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DroneService _droneService;

        private const float MAX_DISTANCE_DEPTH_COLLIDER = 0.032f;
        private const float TIME_FOR_DEAD = 1f;

        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private DroneModel _droneModel;
        private Coroutine _fallingEnergy;
        private bool _isShieldActivate;
        private BoosterDescriptor _speedBoosterDescriptor;
        private BoosterDescriptor _shieldBoosterDescriptor;

        public void Init()
        {
            _dronId = _levelService.SelectedDroneId;
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _overlayManager.Require().HideLoadingOverlay(true);
            _droneModel = new DroneModel(_droneService.GetDronById(_dronId).DroneDescriptor);
            _gestureService.AddAnyTouchHandler(OnAnyTouch, false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, _droneModel));

            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, OnCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_BATTERY, OnTakeBattery);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.FINISHED, OnFinished);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SPEED, OnTakeSpeed);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SHIELD, OnTakeShield);

            CreateDrone(_dronId);
        }

        private void CreateDrone(string dronId)
        {
            GameObject parent = _gameWorld.Require().GetDroneCube();
            GameObject drone = Instantiate(Resources.Load<GameObject>(_droneService.GetDronById(dronId).DroneDescriptor.Prefab));
            _gameWorld.Require().AddGameObject(drone, parent);
            CinemachineVirtualCamera droneCamera = _gameWorld.Require().GetDroneCamera();
            droneCamera.Follow = drone.transform;
            droneCamera.LookAt = drone.transform;
        }

        private void OnAnyTouch(AnyTouch anyTouch)
        {
            if (_isPlay) {
                return;
            }
            _isPlay = true;
            _fallingEnergy = StartCoroutine(FallEnergy());
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.START_FLIGHT));
            _startTime = Time.time;
        }

        public void EndGame()
        {
            _isPlay = false;
            Time.timeScale = 0f;
            _gestureService.RemoveAnyTouchHandler(OnAnyTouch);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
        }

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), _droneModel.countChips, timeInGame,
                                               (int) ((_droneModel.durability / _droneModel.maxDurability) * 100));
            }
        }

        private void OnCrash(WorldEvent worldEvent)
        {
            if (_isShieldActivate) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASH, worldEvent.ContactPoints, worldEvent.ImmersionDepth));
            _droneModel.durability -= ObstacleModel.Damage;
            if (_droneModel.durability <= 0 || worldEvent.ImmersionDepth > MAX_DISTANCE_DEPTH_COLLIDER) {
                _droneModel.durability = 0;
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASHED));
                DroneFailed(FailedReasons.Crashed);
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnTakeChip(WorldEvent worldEvent)
        {
            _droneModel.countChips++;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnTakeBattery(WorldEvent worldEvent)
        {
            _droneModel.energy += BatteryModel.Energy;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnFinished(WorldEvent worldEvent)
        {
            Victory();
        }

        private void OnTakeSpeed(WorldEvent worldEvent)
        {
            if (_speedBoosterDescriptor == null) {
                _speedBoosterDescriptor = _boosterService.Require().GetDescriptorByType(WorldObjectType.SPEED_BOOSTER);
            }

            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SPEED, _speedBoosterDescriptor));
            Invoke(nameof(DisableSpeed), float.Parse(_speedBoosterDescriptor.Params["Duration"]));
        }

        private void DisableSpeed()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SPEED, _speedBoosterDescriptor));
        }

        private void OnTakeShield(WorldEvent worldEvent)
        {
            if (_speedBoosterDescriptor == null) {
                _shieldBoosterDescriptor = _boosterService.Require().GetDescriptorByType(WorldObjectType.SHIELD_BOOSTER);
            }
            _isShieldActivate = true;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SHIELD));
            Invoke(nameof(DisableShield), float.Parse(_shieldBoosterDescriptor.Params["Duration"]));
        }

        private void DisableShield()
        {
            _isShieldActivate = false;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SHIELD));
        }

        private void Victory()
        {
            SetStatsInProgress(true);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DroneFailed(FailedReasons reason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(reason);
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
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
                    StopCoroutine(_fallingEnergy);
                } else {
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}