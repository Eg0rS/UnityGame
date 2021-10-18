using System;
using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using IoC.Attribute;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Drone;
using Drone.Location.World.Drone.Event;
using Drone.World;
using Drone.World.Event;
using IoC.Util;

namespace Drone.Location.World.Dron
{
    public class testDronControllerPryanik : GameEventDispatcher, IWorldObjectController<DronePrefabModel>
    {
        private const float UPDATE_TIME = 0.1f;
        
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        private testDronControlServicePryanik _dronControlService;
        
        public WorldObjectType ObjectType { get; }
        
        private float _acceleration = 0.2f;
        private float _maxSpeed;
        private float _shiftSpeed = 0.13f;
        private BezierWalkerWithSpeed _bezier;
        private bool _isGameRun;
        private Coroutine _isMoving;
        private Vector3 _droneTargetPosition = Vector3.zero;

        public Vector2 _Vector2;
        
        public void Init(DronePrefabModel model)
        {
            _dronControlService = gameObject.AddComponent<testDronControlServicePryanik>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _bezier.speed = 0;
            // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, SpeedBoost);
            // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            // _dronControlService.AddListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            // _dronControlService.AddListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, SetParameters);
            // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, Deceleration);
            _shiftSpeed = 2f;
            _maxSpeed = 10;
            _acceleration = 1.5f;
            StartGame(new WorldEvent(WorldEvent.START_FLIGHT));
            OnStart(new ControllEvent(ControllEvent.START_MOVE, Vector2.zero));
        }

        private void Start()
        {
            Init(new DronePrefabModel());
        }

        private void SetParameters(WorldEvent worldEvent)
        {
            _maxSpeed = worldEvent.DroneModel.maxSpeed;
            _acceleration = worldEvent.DroneModel.acceleration;
        }
        
        private void StartGame(WorldEvent worldEvent)
        {
            _isGameRun = true;
            _bezier.enabled = true;
        }
        
        public void Update()
        {
            OnSwiped( new ControllEvent(ControllEvent.START_MOVE,_Vector2));
            if (!_isGameRun) {
                return;
            }
            
            if (_bezier.speed < _maxSpeed) {
                _bezier.speed += _acceleration * Time.deltaTime;
            }
            else if (_bezier.speed > _maxSpeed) {
                _bezier.speed -= _acceleration * Time.deltaTime;
            }
        }
        
        private void EndGame(WorldEvent worldEvent)
        {
            _dronControlService.RemoveListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _dronControlService.RemoveListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.ENABLE_SPEED, SpeedBoost);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }
        
        private void OnStart(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            Debug.Log(_droneTargetPosition);
            MoveTo(newPosition);
        }

        private void OnSwiped(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
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
            _isMoving = StartCoroutine(Moving(newPos));
        }
        
        private IEnumerator Moving(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 move = targetPosition - startPosition;
            float distance = (move).magnitude;
            float time = distance / _shiftSpeed;
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
        
        private void OnCollisionEnter(Collision other)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ON_COLLISION, other.gameObject));
        }
        
        private void Deceleration(WorldEvent objectEvent)
        {
            _bezier.speed /= 2;
        }
        private void SpeedBoost(WorldEvent objectEvent)
        {
            _maxSpeed *= float.Parse(objectEvent.SpeedBooster.Params["SpeedBoost"]);
            _acceleration *= float.Parse(objectEvent.SpeedBooster.Params["AccelerationBoost"]);;
            StartCoroutine(DisableSpeedBoost(objectEvent));
        }
        
        private IEnumerator DisableSpeedBoost(WorldEvent objectEvent)
        {
            yield return new WaitForSeconds(float.Parse(objectEvent.SpeedBooster.Params["Duration"]));
            _maxSpeed /= float.Parse(objectEvent.SpeedBooster.Params["SpeedBoost"]);
            _acceleration /= float.Parse(objectEvent.SpeedBooster.Params["AccelerationBoost"]);
        }
    }
}