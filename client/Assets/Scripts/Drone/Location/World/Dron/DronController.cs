using System;
using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using IoC.Attribute;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Drone;
using Drone.Location.Service;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.Event;
using Drone.Location.World.Dron.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Util;

namespace Drone.Location.World.Dron
{
    public class DronController : GameEventDispatcher, IWorldObjectController<DroneModel>
    {
        private const float UPDATE_TIME = 0.1f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private DronService _dronService;

        [Inject]
        private GameService _gameService;

        private DronControlService _dronControlService;

        public WorldObjectType ObjectType { get; }

        private float _acceleration = 0.2f;
        private float _maxSpeed;
        private float _shiftSpeed = 0.13f;
        private BezierWalkerWithSpeed _bezier;
        private bool _isGameRun;
        private float _boostSpeed;
        private Coroutine _isMoving;
        private Vector3 _droneTargetPosition = Vector3.zero;

        private Animator _animator;
        private int _prevDirection;
        private bool _isIs;

        public void Init(DroneModel model)
        {
            _dronControlService = gameObject.AddComponent<DronControlService>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _bezier.speed = 0;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, SpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _dronControlService.AddListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _dronControlService.AddListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, SetParameters);
            DronDescriptor dronDescriptor = _dronService.GetDroneById(_gameService.DronId).DroneDescriptor;
            model.SetDroneParameters(dronDescriptor.Mobility, dronDescriptor.Durability, dronDescriptor.Energy);
            _shiftSpeed = model.Mobility;
            _animator = GetComponent<Animator>();
            _animator.speed /= _shiftSpeed;
        }

        private void SetParameters(WorldEvent worldEvent)
        {
            _maxSpeed = worldEvent.DronParams.maxSpeed;
            _acceleration = worldEvent.DronParams.acceleration;
        }

        private void StartGame(WorldEvent worldEvent)
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
            }
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _dronControlService.RemoveListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _dronControlService.RemoveListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, SpeedBoost);
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

            int direction = GetMoveDirection(targetPosition);

            Debug.Log(_prevDirection);
            _animator.SetInteger("moveDirection", targetPosition == Vector3.zero ? GetMoveDirection(-startPosition) : direction);

            // if (targetPosition == Vector3.zero) {
            //     _isIdleState = true;
            //     _animator.SetBool("isIdleState", _isIdleState);
            //     _animator.SetInteger("moveDirection", GetMoveDirection(-startPosition));
            // } else {
            //     _animator.SetBool("isIdleState", _isIdleState);
            //     _animator.SetInteger("moveDirection", direction);
            // }

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
                    _animator.SetInteger("moveDirection", 0);
                    _droneTargetPosition = targetPosition;
                }

                yield return 0.1;
            }
            _prevDirection = direction;
            _isMoving = null;
        }

        private int GetMoveDirection(Vector2 position)
        {
            int result = 0;
            if (position == Vector2.up) {
                result = 1;
            }
            if (position == Vector2.down) {
                result = 2;
            }
            if (position == Vector2.left) {
                result = 3;
            }
            if (position == Vector2.right) {
                result = 4;
            }
            
            
            if (position == new Vector2(-1, 1) && (_prevDirection != 1 || _prevDirection != 3)) { //(_prevDirection != 1 || _prevDirection != 3)
                result = 5;
            }
            if (position == new Vector2(-1, -1) && (_prevDirection != 2 || _prevDirection != 3)) { //(_prevDirection != 2 || _prevDirection != 3)
                result = 6;
            }
            if (position == new Vector2(1, -1) && (_prevDirection != 2 || _prevDirection != 4)) { //(_prevDirection != 2 || _prevDirection != 4)
                result = 7;
            }
            if (position == new Vector2(1, 1) && (_prevDirection != 1 || _prevDirection != 4)) { //(_prevDirection != 1 || _prevDirection != 4)
                result = 8;
            }

            return result;
        }

        // private int GetMoveDirection(Vector2 position) //&& _prevDirection == 0
        // {
        //     if ((position == Vector2.up && _prevDirection == 0)) { //|| (_prevDirection == 2 && position == Vector2.zero)
        //         //(_prevDirection != 5 && _prevDirection != 8)
        //         return 1;
        //     }
        //     if ((position == Vector2.down && _prevDirection == 0)) { //|| (_prevDirection == 1 && position == Vector2.zero)
        //         //(_prevDirection != 6 && _prevDirection != 7)
        //         return 2;
        //     }
        //     switch (_prevDirection) {
        //         case 1 when position == new Vector2(-1, 1):
        //         case 8 when position == Vector2.up:
        //         case 0 when position == Vector2.left:
        //         case 4 when position == Vector2.zero:
        //         case 2 when position == new Vector2(-1, -1):
        //         case 7 when position == Vector2.down:
        //             return 3;
        //         case 5 when position == Vector2.up:
        //         case 1 when position == new Vector2(1, 1):
        //         case 3 when position == Vector2.zero:
        //         case 0 when position == Vector2.right:
        //         case 6 when position == Vector2.down:
        //         case 2 when position == new Vector2(1, -1):
        //             return 4;
        //     }
        //
        //     if (position == new Vector2(-1, 1) && (_prevDirection != 1 || _prevDirection != 3)) { //(_prevDirection != 1 || _prevDirection != 3)
        //         return 5;
        //     }
        //     if (position == new Vector2(-1, -1) && (_prevDirection != 2 || _prevDirection != 3)) { //(_prevDirection != 2 || _prevDirection != 3)
        //         return 6;
        //     }
        //     if (position == new Vector2(1, -1) && (_prevDirection != 2 || _prevDirection != 4)) { //(_prevDirection != 2 || _prevDirection != 4)
        //         return 7;
        //     }
        //     if (position == new Vector2(1, 1) && (_prevDirection != 1 || _prevDirection != 4)) { //(_prevDirection != 1 || _prevDirection != 4)
        //         return 8;
        //     }
        //
        //     return 0;
        // }

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
            _maxSpeed += objectEvent.SpeedBoost;
            _acceleration += objectEvent.AccelerationBoost;
        }
    }
}