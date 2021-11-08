using AgkCommons.Event;
using BezierSolution;
using Cinemachine;
using DG.Tweening;
using Drone.Location.Model;
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

        private const float MINIMAL_SPEED = 3.0f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private DroneAnimationController _droneAnimationController;

        private float _acceleration = 0.2f;
        private float _maxSpeed;
        private float _basemobility;
        private float _mobility;
        private BezierWalkerWithSpeed _bezier;
        private Coroutine _isMoving;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private float _minimalSpeed = 3.0f;
        private bool _isGameRun;
        public WorldObjectType ObjectType { get; }
        private float _crashNoise = 2;
        private float _crashNoiseDuration = 0.5f;
        
        private Sequence _sequence;

        public void Init(DronePrefabModel model)
        {
            _droneAnimationController = gameObject.AddComponent<DroneAnimationController>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _cameraNoise = _gameWorld.Require().GetDroneCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, OnSetParameters);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, OnStartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SPEED, OnEnableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SPEED, OnDisableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, OnEnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, OnDisableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASH, OnCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASHED, OnCrashed);
            
            _sequence = DOTween.Sequence();
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
            _mobility = _basemobility * (MINIMAL_SPEED / _bezier.speed);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.localRotation.y, _droneTargetPosition.x - newPos.x) * 45;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility))
                     .Join(transform.DOLocalRotate(rotation, _mobility).OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility); }));
        }

        private void OnSetParameters(WorldEvent worldEvent)
        {
            _bezier.enabled = false;
            _bezier.speed = _minimalSpeed;
            _maxSpeed = worldEvent.DroneModel.maxSpeed;
            _acceleration = worldEvent.DroneModel.acceleration;
            _basemobility = worldEvent.DroneModel.mobility;
        }

        private void OnStartGame(WorldEvent worldEvent)
        {
            _isGameRun = true;
            _bezier.enabled = true;
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
            _cameraNoise.m_AmplitudeGain += _crashNoise;
            Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
        }

        private void OnCrashed(WorldEvent worldEvent)
        {
            _bezier.enabled = false;
            _droneAnimationController.OnCrashed();
            gameObject.SetActive(false);
            _cameraNoise.m_AmplitudeGain += _crashNoise;
            Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= _crashNoise;
        }

        private void OnEnableSpeedBoost(WorldEvent worldEvent)
        {
            _maxSpeed *= float.Parse(worldEvent.SpeedBooster.Params["SpeedBoost"]);
            _acceleration *= float.Parse(worldEvent.SpeedBooster.Params["AccelerationBoost"]);
            _droneAnimationController.EnableSpeedBoost();
        }

        private void OnDisableSpeedBoost(WorldEvent worldEvent)
        {
            _maxSpeed /= float.Parse(worldEvent.SpeedBooster.Params["SpeedBoost"]);
            _acceleration /= float.Parse(worldEvent.SpeedBooster.Params["AccelerationBoost"]);
            _droneAnimationController.DisableSpeedBoost();
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