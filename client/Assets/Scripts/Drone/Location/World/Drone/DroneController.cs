using System.Collections;
using AgkCommons.Event;
using AgkCommons.Extension;
using BezierSolution;
using Cinemachine;
using DG.Tweening;
using Drone.Location.Model;
using Drone.Location.Event;
using Drone.Location.Model.Drone;
using Drone.Location.World.Drone.Event;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneController : GameEventDispatcher, IWorldObjectController<DronePrefabModel>
    {
        public WorldObjectType ObjectType { get; }
        private const float MINIMAL_SPEED = 3.0f;
        private const float CRASH_NOISE = 2;
        private const float CRASH_NOISE_DURATION = 0.5f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private DroneAnimationController _droneAnimationController;

        private BezierWalkerWithSpeed _bezier;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;
        private CinemachineVirtualCamera _camera;

        private float _acceleration;
        private float _maxSpeed;
        private float _baseMobility;
        private float _mobility;

        private Vector3 _droneTargetPosition = Vector3.zero;
        private bool _isGameRun;
        private Sequence _sequence;

        public void Init(DronePrefabModel model)
        {
            _droneAnimationController = gameObject.AddComponent<DroneAnimationController>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _cameraNoise = _gameWorld.Require().GetDroneCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, OnSetParameters);
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.START_GAME, OnStartGame);
            _gameWorld.Require().AddListener<AcceleratorEvent>(AcceleratorEvent.ACCELERATION, OnAccelerator);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, OnEnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, OnDisableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASH, OnCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASHED, OnCrashed);
            _camera = _gameWorld.Require().GetDroneCamera();
            _sequence = DOTween.Sequence();
        }

        private void OnAccelerator(AcceleratorEvent acceleratorEvent)
        {
            StartCoroutine(AcceleratorCorutine(acceleratorEvent.AcceleratorModel.AccelerationDuration, acceleratorEvent.AcceleratorModel.AccelerationForce));
        }

        private IEnumerator AcceleratorCorutine(float duration, float force)
        {
            _maxSpeed *= force;
            _acceleration *= force;
            _droneAnimationController.EnableSpeedBoost();
            
            yield return new WaitForSeconds(duration);
            
            _maxSpeed /= force;
            _acceleration /= force;
            _droneAnimationController.DisableSpeedBoost();
        }

        private void OnGesture(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Gesture.x, objectEvent.Gesture.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            DotWeenMove(newPosition);
        }

        private Vector3 NewPosition(Vector3 dronPos, Vector3 swipe)
        {
            Vector3 newPos = dronPos + swipe;
            if (newPos.x > 1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.x < -1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.y > 1.0f) {
                swipe.y = 0.0f;
            }
            if (newPos.y < -1.0f) {
                swipe.y = 0.0f;
            }
            Vector3 newPosition = dronPos + swipe;
            return newPosition;
        }

        private void DotWeenMove(Vector3 newPos)
        {
            _mobility = _baseMobility * (MINIMAL_SPEED / _bezier.speed);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.localRotation.y, _droneTargetPosition.x - newPos.x) * 45;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility))
                     .Join(transform.DOLocalRotate(rotation, _mobility).OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility); }));
        }

        private void OnSetParameters(WorldEvent worldEvent)
        {
            _bezier.speed = 0;
            _maxSpeed = worldEvent.DroneModel.maxSpeed;
            _acceleration = worldEvent.DroneModel.acceleration;
            _baseMobility = worldEvent.DroneModel.mobility;
            CreateDrone(worldEvent.DroneModel.DroneDescriptor.Prefab);
        }

        private void CreateDrone(string droneDescriptorPrefab)
        {
            GameObject drone = Instantiate(Resources.Load<GameObject>(droneDescriptorPrefab));
            _gameWorld.Require().AddGameObject(drone, gameObject.GetChildren().Find(x => x.name == "Mesh"));
            CinemachineVirtualCamera droneCamera = _gameWorld.Require().GetDroneCamera();
            droneCamera.Follow = _gameWorld.Require().GetPlayerContainer().transform;
            droneCamera.LookAt = transform;
        }

        private void OnStartGame(ControllEvent controllEvent)
        {
            _bezier.speed = MINIMAL_SPEED;
            _isGameRun = true;
        }

        public void Update()
        {
            if (!_isGameRun) {
                return;
            }
            SetBezierSpeed();
        }

        private void SetBezierSpeed()
        {
            if (_bezier.speed >= _maxSpeed) {
                return;
            }
            float newSpeed = _bezier.speed + _acceleration * Time.deltaTime;
            _bezier.speed = newSpeed > _maxSpeed ? _maxSpeed : newSpeed;
        }

        private void OnCrash(WorldEvent worldEvent)
        {
            _bezier.speed /= 2;
            _droneAnimationController.OnCrash(worldEvent);
            _cameraNoise.m_AmplitudeGain += CRASH_NOISE;
            Invoke(nameof(DisableCrashNoise), CRASH_NOISE_DURATION);
        }

        private void OnCrashed(WorldEvent worldEvent)
        {
            _bezier.enabled = false;
            _droneAnimationController.OnCrashed();
            gameObject.SetActive(false);
            _cameraNoise.m_AmplitudeGain += CRASH_NOISE;
            Invoke(nameof(DisableCrashNoise), CRASH_NOISE_DURATION);
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= CRASH_NOISE;
        }

        private void OnEnableShield(WorldEvent worldEvent)
        {
            _droneAnimationController.EnableShield();
        }

        private void OnDisableShield(WorldEvent worldEvent)
        {
            _droneAnimationController.DisableShield();
        }
    }
}