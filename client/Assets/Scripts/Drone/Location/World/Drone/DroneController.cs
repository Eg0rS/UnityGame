using System;
using System.Collections;
using AgkCommons.Event;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Service;
using Drone.Location.World.Drone.Descriptor;
using Drone.Location.World.Drone.Event;
using Drone.Location.World.Drone.Model;
using Drone.Location.World.Drone.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneController : GameEventDispatcher, IWorldObjectController<DroneModel>
    {
        private const float UPDATE_TIME = 0.1f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private DroneService _droneService;

        [Inject]
        private GameService _gameService;

        private DronControlService _droneControlService;

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
        private bool _alreadyAnimationBegin;

        public void Init(DroneModel model)
        {
            _droneControlService = gameObject.AddComponent<DronControlService>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _bezier.speed = 0;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, SpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, SetParameters);
            DronDescriptor dronDescriptor = _droneService.GetDronById(_gameService.DronId).DronDescriptor;
            _animator = GetComponent<Animator>();
            _animator.speed *= _shiftSpeed * 5; //TOdo какую-нибудь формулу расчёта скорости анимации из скорости дрона
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
            if (!_isGameRun) {
                return;
            }

            if (_bezier.speed < _maxSpeed) {
                _bezier.speed += _acceleration * Time.deltaTime;
            }
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _droneControlService.RemoveListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _droneControlService.RemoveListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
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
            _alreadyAnimationBegin = true;
            _isMoving = StartCoroutine(Moving(newPos));
        }

        private IEnumerator Moving(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 move = targetPosition - startPosition;

            if (_alreadyAnimationBegin) {
                int direction = GetMoveDirection(targetPosition);
                _animator.SetInteger("moveDirection", targetPosition == Vector3.zero ? GetMoveDirection(-startPosition) : direction);
                _alreadyAnimationBegin = false;
            }

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
            _isMoving = null;
        }

        private int GetMoveDirection(Vector2 position)
        {
            Vector2 currentPos = transform.localPosition;
            Vector2 move = position - currentPos;

            if ((move.x > 0 && move.x < 1) || move.x > 1) {
                move.x = 1;
            }
            if (move.x < -1 || (move.x > -1 && move.x < 0)) {
                move.x = -1;
            }

            if ((move.y > 0 && move.y < 1) || move.y > 1) {
                move.y = 1;
            }
            if (move.y < -1 || (move.y > -1 && move.y < 0)) {
                move.y = -1;
            }
            
            if (move == Vector2.up) {
                return 1;
            }
            if (move == Vector2.down) {
                return 2;
            }
            if (move == Vector2.left) {
                return 3;
            }
            if (move == Vector2.right) {
                return 4;
            }
            
            
            if (position == new Vector2(-1, 1)) {
                return 5;
            }
            if (position == new Vector2(-1, -1)) {
                return 6;
            }
            if (position == new Vector2(1, -1)) {
                return 7;
            }
            if (position == new Vector2(1, 1)) {
                return 8;
            }
            
            return 0;
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
            _maxSpeed += objectEvent.SpeedBoost;
            _acceleration += objectEvent.AccelerationBoost;
        }
    }
}