using System.Collections;
using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Dialog.Service;
using Cinemachine;
using Drone.Core;
using Drone.Core.Service;
using Drone.LevelMap.LevelDialogs;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Service;
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
        private IGestureService _gestureService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DroneService _droneService;

        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private DroneModel _droneModel;
        private Coroutine _fallingEnergy;

        public void Init()
        {
            _dronId = _levelService.SelectedDroneId;
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _overlayManager.Require().HideLoadingOverlay(true);
            _droneModel = new DroneModel(_droneService.GetDronById(_dronId).DroneDescriptor);
            _gestureService.AddAnyTouchHandler(OnAnyTouch, false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, _droneModel));
            
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.VICTORY, Victory);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASHED, Crashed);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, OnCrush);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_BATTERY, OnTakeBattery);

            CreateDrone(_dronId);
        }

        private void Crashed(WorldEvent obj)
        {
            DroneFailed(obj.FailedReason);
        }

        private void OnTakeBattery(WorldEvent obj)
        {
            _droneModel.energy += BatteryModel.Energy;
        }

        private void OnTakeChip(WorldEvent obj)
        {
            _droneModel.countChips++;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnCrush(WorldEvent worldEvent)
        {
            _droneModel.durability -= ObstacleModel.Damage;
            if (_droneModel.durability <= 0) {
                _droneModel.durability = 0;
                DroneFailed(FailedReasons.Crashed);
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
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

        private void Victory(WorldEvent worldEvent)
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
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED, FailedReasons.EnergyFalled));
                    StopCoroutine(_fallingEnergy);
                } else {
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}