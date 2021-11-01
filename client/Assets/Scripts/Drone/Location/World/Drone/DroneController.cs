using System;
using System.Collections;
using AgkCommons.Event;
using BezierSolution;
using Cinemachine;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Drone;
using Drone.Location.World.BonusChips;
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
        private const float UPDATE_TIME = 0.1f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private DroneAnimationController _droneAnimationController;

        private float _acceleration = 0.2f;
        private float _maxSpeed;
        private float _basemobility;
        private float _mobility;
        private DroneControlService _droneControlService;
        private BezierWalkerWithSpeed _bezier;
        private Coroutine _isMoving;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private Vector3 _dronePreviosPosition = Vector3.zero;
        private float _minimalSpeed = 3.0f;
        private bool _isGameRun;
        public WorldObjectType ObjectType { get; }
        private float _crashNoise = 2;
        private float _crashNoiseDuration = 0.5f;

        public void Init(DronePrefabModel model)
        {
            _droneControlService = gameObject.AddComponent<DroneControlService>();
            _droneAnimationController = gameObject.AddComponent<DroneAnimationController>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _cameraNoise = _gameWorld.Require().GetDroneCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, OnSetParameters);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, OnStartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SPEED, OnEnableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SPEED, OnDisableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, OnEnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, OnDisableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASH, OnCrash);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRONE_CRASHED, OnCrashed);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
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
            if (_bezier.speed < _maxSpeed) {
                _bezier.speed += _acceleration * Time.deltaTime;
            } else if (_bezier.speed > _maxSpeed) {
                _bezier.speed -= _acceleration * Time.deltaTime;
            }
            _mobility = _basemobility * (_bezier.speed / _minimalSpeed);
        }

        private void OnStart(ControllEvent worldEvent)
        {
            Vector3 swipe = new Vector3(worldEvent.Swipe.x, worldEvent.Swipe.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            _dronePreviosPosition = _droneTargetPosition;
            MoveTo(newPosition);
        }

        private void OnSwiped(ControllEvent worldEvent)
        {
            Vector3 swipe = new Vector3(worldEvent.Swipe.x, worldEvent.Swipe.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            _dronePreviosPosition = _droneTargetPosition;
            _droneTargetPosition = newPosition;
            Debug.Log(_droneTargetPosition);
            MoveTo(newPosition);
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

        private void MoveTo(Vector3 newPos)
        {
            if (_isMoving != null) {
                StopCoroutine(_isMoving);
            }
            _droneAnimationController.MoveTo(newPos - _dronePreviosPosition, newPos, _mobility);
            _isMoving = StartCoroutine(Moving(newPos));
        }

        private IEnumerator Moving(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 move = targetPosition - startPosition;
            float distance = (move).magnitude;
            float time = distance / _mobility;
            float updateCount = (float) Math.Ceiling(time / UPDATE_TIME);
            float deltaX = move.x / updateCount;
            float deltaY = move.y / updateCount;
            bool complete = false;

            bool upDirection = targetPosition.y > startPosition.y;
            bool rightDirection = targetPosition.x > startPosition.x;

            while (!complete) {
                Vector3 currentPosition = transform.localPosition;
                float xPos = currentPosition.x += deltaX;
                if (rightDirection) {
                    if (xPos > targetPosition.x) {
                        xPos = targetPosition.x;
                    }
                } else {
                    if (xPos < targetPosition.x) {
                        xPos = targetPosition.x;
                    }
                }

                float yPos = currentPosition.y += deltaY;
                if (upDirection) {
                    if (yPos > targetPosition.y) {
                        yPos = targetPosition.y;
                    }
                } else {
                    if (yPos < targetPosition.y) {
                        yPos = targetPosition.y;
                    }
                }

                transform.localPosition = new Vector3(xPos, yPos, 0);

                if (targetPosition.Equals(transform.localPosition)) {
                    complete = true;
                    _droneTargetPosition = targetPosition;
                }
                yield return 0.1;
            }
            _isMoving = null;
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