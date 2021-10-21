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
using Drone.Location.Model.Finish;
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
        private DroneService _droneService;

        [Inject]
        private LocationService _locationService;

        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;

        public DroneModel DroneModel { get; private set; }

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
            DroneModel = new DroneModel(_droneService.GetDronById(_dronId).DroneDescriptor);
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
            _gestureService.AddAnyTouchHandler(OnAnyTouch, false);
            Dispatch(new WorldEvent(WorldEvent.WORLD_CREATED, DroneModel));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.SET_DRON_PARAMETERS, DroneModel));
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_FAILED, DroneFailed);
            CreateDrone(_dronId);
        }

        private void CreateDrone(string dronId)
        {
            GameObject parent = _gameWorld.Require().GetDroneCube();
            GameObject drone = Instantiate(Resources.Load<GameObject>(_droneService.GetDronById(dronId).DroneDescriptor.Prefab));
            _gameWorld.Require().AddGameObject(drone, parent);
            CinemachineVirtualCamera camera = _gameWorld.Require().GetDroneCamera();
            camera.Follow = drone.transform;
            camera.LookAt = drone.transform;
        }

        private void OnAnyTouch(AnyTouch anyTouch)
        {
            if (_isPlay) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.START_FLIGHT));
            _startTime = Time.time;
        }

        private void OnDronCollision(WorldEvent worldEvent)
        {
            Collision collisionObject = worldEvent.CollisionObject;
            switch (collisionObject.gameObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.FINISH:
                    Victory(collisionObject.gameObject.GetComponent<FinishModel>());
                    break;
            }
        }

        public void EndGame()
        {
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
                _levelService.SetLevelProgress(_levelService.SelectedLevelId, CalculateStars(timeInGame), DroneModel.countChips, timeInGame,
                                               (int) ((DroneModel.durability / DroneModel.maxDurability) * 100));
            }
        }

        private void Victory(FinishModel component)
        {
            SetStatsInProgress(true);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFinishedDialog>();
        }

        private void DroneFailed(WorldEvent worldEvent)
        {
            FailedReasons failedReason = worldEvent.FailedReason;
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(failedReason);
        }

        private int CalculateStars(float timeInGame)
        {
            int countStars = 0;

            if (DroneModel.durability >= _levelDescriptor.NecessaryDurability) {
                countStars++;
            }
            if (DroneModel.countChips >= _levelDescriptor.NecessaryCountChips) {
                countStars++;
            }
            if (timeInGame <= _levelDescriptor.NecessaryTime) {
                countStars++;
            }

            return countStars;
        }
    }
}