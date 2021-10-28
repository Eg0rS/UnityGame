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

        private const float TIME_FOR_DEAD = 1f;

        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private DroneModel _droneModel;
        private Coroutine _fallingEnergy;
        private int _batteryEnergy;

        public void Init()
        {
            _dronId = _levelService.SelectedDroneId;
            _levelDescriptor = _levelService.GetLevelDescriptorById(_levelService.SelectedLevelId);
            _overlayManager.Require().HideLoadingOverlay(true);
            _droneModel = new DroneModel(_droneService.GetDronById(_dronId).DroneDescriptor);
            _gestureService.AddAnyTouchHandler(OnAnyTouch, false);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, _droneModel));
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_CHIP, OnTakeChip);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_BATTERY, OnTakeBattery);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.FINISHED, OnFinished);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASH, OnDroneCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_LETHAL_CRASH, OnDroneLethalCrash);
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

        private void SetStatsInProgress(bool isWin)
        {
            float timeInGame = Time.time - _startTime;
            if (isWin) {
                _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), _droneModel.countChips, timeInGame,
                                               (int) ((_droneModel.durability / _droneModel.maxDurability) * 100));
            }
        }

        private void OnDroneCrash(WorldEvent worldEvent)
        {
            _droneModel.durability -= worldEvent.Damage;
            if (_droneModel.durability <= 0) {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED));
                Invoke(nameof(Crashed), TIME_FOR_DEAD);
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnDroneLethalCrash(WorldEvent obj)
        {
            _droneModel.durability = 0;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED));
            Invoke(nameof(Crashed), TIME_FOR_DEAD);
        }

        private void Crashed()
        {
            DroneFailed(FailedReasons.Crashed);
        }

        private void OnTakeChip(WorldEvent worldEvent)
        {
            _droneModel.countChips++;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnTakeBattery(WorldEvent worldEvent)
        {
            _droneModel.energy += _batteryEnergy;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void OnFinished(WorldEvent worldEvent)
        {
            Victory();
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

        public void EndGame()
        {
            _isPlay = false;
            Time.timeScale = 0f;
            _gestureService.RemoveAnyTouchHandler(OnAnyTouch);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.END_GAME));
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
                    DroneFailed(FailedReasons.EnergyFalled);
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