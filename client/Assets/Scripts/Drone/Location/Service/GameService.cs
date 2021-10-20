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
using Drone.Location.World.Drone;
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

        [Inject]
        private IoCProvider<DroneAnimService> _droneAnimService;

        private LevelDescriptor _levelDescriptor;
        private bool _isPlay;
        private string _dronId;
        private float _startTime;
        private bool _onActiveShield;
        private Coroutine _fallingEnergy;
        private float _crashNoise = 2;
        private float _crashNoiseDuration = 0.5f;

        private ShieldBoosterModel _shieldBoosterModel;
        private SpeedBoosterModel _speedBoosterModel;

        private CinemachineBasicMultiChannelPerlin _cameraNoise;

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
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, EnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, DisableShield);
            CreateDrone(_dronId);
        }

        private void CreateDrone(string dronId)
        {
            GameObject parent = _gameWorld.Require().GetGameObjectByName("DroneCube");
            GameObject drone = Instantiate(Resources.Load<GameObject>(_droneService.GetDronById(dronId).DroneDescriptor.Prefab));
            _gameWorld.Require().AddGameObject(drone, parent);
            CinemachineVirtualCamera camera = _gameWorld.Require().GetGameObjectByName("CM vcam1")?.GetComponent<CinemachineVirtualCamera>();
            _cameraNoise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            camera.Follow = drone.transform;
            camera.LookAt = drone.transform;
        }

        private void EnableShield(WorldEvent obj)
        {
            _onActiveShield = true;
        }

        private void DisableShield(WorldEvent obj)
        {
            _onActiveShield = false;
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
            Collision collisionObject = worldEvent.CollisionObject;
            switch (collisionObject.gameObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(collisionObject.gameObject.GetComponent<ObstacleModel>(), collisionObject.contacts);
                    break;
                case WorldObjectType.BATTERY:
                    OnTakeBattery(collisionObject.gameObject.GetComponent<BatteryModel>());
                    break;
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(collisionObject.gameObject.GetComponent<BonusChipsModel>());
                    break;
                case WorldObjectType.FINISH:
                    Victory(collisionObject.gameObject.GetComponent<FinishModel>());
                    break;
            }
        }

        private void OnTakeBattery(BatteryModel component)
        {
            DroneModel.energy += component.Energy;
        }

        private void OnTakeChip(BonusChipsModel component)
        {
            DroneModel.countChips++;
            UiUpdate();
        }

        private void OnDronCrash(ObstacleModel component, ContactPoint[] contactPoints)
        {
            if (_onActiveShield) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASH));
            _cameraNoise.m_AmplitudeGain += _crashNoise;
            foreach (ContactPoint contact in contactPoints) {
                _droneAnimService.Require().PlayParticleState(DroneParticles.ptSparks, contact.point, component.transform.rotation);
            }
            Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
            DroneModel.durability -= component.Damage;
            if (DroneModel.durability <= 0) {
                DroneModel.durability = 0;
                DronFailed(FailedReasons.Crashed);
            }
            UiUpdate();
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= _crashNoise;
        }

        private void UiUpdate()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, DroneModel));
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

        private void DronFailed(FailedReasons failReason)
        {
            SetStatsInProgress(false);
            EndGame();
            _dialogManager.Require().ShowModal<LevelFailedCompactDialog>(failReason);
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

        private IEnumerator FallEnergy()
        {
            while (_isPlay) {
                DroneModel.energy -= DroneModel.energyFall;
                if (DroneModel.energy <= 0) {
                    DroneModel.energy = 0;
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