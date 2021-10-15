using System;
using System.Collections;
using AgkCommons.Event;
using BezierSolution;
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
        private const float UPDATE_TIME = 0.1f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        private float _acceleration = 0.2f;
        private float _maxSpeed;
        private float _basemobility;
        private float _mobility;
        private DroneControlService _droneControlService;
        private BezierWalkerWithSpeed _bezier;
        private Coroutine _isMoving;
        private Animator _animator;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private float _minimalSpeed = 3.0f;
        private bool _isGameRun;
        private bool _animationAlreadyBegin;
        public WorldObjectType ObjectType { get; }

        public void Init(DronePrefabModel model)
        {
            _droneControlService = gameObject.AddComponent<DroneControlService>();
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _droneControlService = gameObject.AddComponent<DroneControlService>();
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SPEED, EnableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SPEED, DisableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _droneControlService.AddListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, SetParameters);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, Deceleration);
            _animator = GetComponent<Animator>();
            //_animator.speed *= ANIMATION_SPEED; TODO какую-нибудь формулу
        }

        private void SetParameters(WorldEvent worldEvent)
        {
            _bezier.enabled = false;
            _bezier.speed = _minimalSpeed;
            _maxSpeed = worldEvent.DroneModel.maxSpeed;
            _acceleration = worldEvent.DroneModel.acceleration;
            _basemobility = worldEvent.DroneModel.mobility;
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
            } else if (_bezier.speed > _maxSpeed) {
                _bezier.speed -= _acceleration * Time.deltaTime;
            }
            _mobility = _basemobility * (_bezier.speed / _minimalSpeed);
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _droneControlService.RemoveListener<ControllEvent>(ControllEvent.START_MOVE, OnStart);
            _droneControlService.RemoveListener<ControllEvent>(ControllEvent.END_MOVE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.ENABLE_SPEED, EnableSpeedBoost);
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
            _animationAlreadyBegin = false;
            _animator.SetInteger("moveDirection", 0);
            if (_isMoving != null) {
                StopCoroutine(_isMoving);
            }
            _isMoving = StartCoroutine(Moving(newPos));
            _animationAlreadyBegin = true;
        }

        private IEnumerator Moving(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 move = targetPosition - startPosition;
            if (!_animationAlreadyBegin) {
                int direction = GetMoveDirection(startPosition, targetPosition);
                _animator.SetInteger("moveDirection", direction);
            }
            float distance = (move).magnitude;
            float time = distance / _mobility;
            float updateCount = (float) Math.Ceiling(time / UPDATE_TIME);
            float deltaX = move.x / updateCount;
            float deltaY = move.y / updateCount;
            bool complete = false;

            bool upDirection = targetPosition.y > startPosition.y;
            bool rightDirection = targetPosition.x > startPosition.x;

            int countMovement = 0;
            while (!complete) {
                countMovement++;
                Debug.Log(countMovement + "CountMovement");
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
                    _animator.SetInteger("moveDirection", 0);
                }
                if (countMovement >= 75) {
                    transform.localPosition = targetPosition;
                }
                yield return 0.1;
            }
            _isMoving = null;
        }

        private int GetMoveDirection(Vector2 startPos, Vector2 direction)
        {
            Vector2 move = direction - startPos;
            move = RoundMoveVector(move);
            
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

            if (move == new Vector2(-1, 1)) {
                return 5;
            }
            if (move == new Vector2(-1, -1)) {
                return 6;
            }
            if (move == new Vector2(1, -1)) {
                return 7;
            }
            if (move == new Vector2(1, 1)) {
                return 8;
            }

            return 0;
        }

        private Vector2 RoundMoveVector(Vector2 move)
        {
            if (move.x > 0 && move.x < 0.5f) {
                move.x = 0;
            }
            if (move.y > 0 && move.y < 0.5f) {
                move.y = 0;
            }
            if (move.x > -0.5f && move.x < 0) {
                move.x = 0;
            }
            if (move.y > -0.5f && move.y < 0) {
                move.y = 0;
            }

            if (move.x > 1 || (move.x > 0.5f && move.x < 1)) {
                move.x = 1;
            }
            if (move.y > 1 || (move.y > 0.5f && move.y < 1)) {
                move.y = 1;
            }
            if ((move.x < -1 && move.x != 0) || (move.x > -1 && move.x < 0)) {
                move.x = -1;
            }
            if (move.y < -1 || (move.y > -1 && move.y < 0)) {
                move.y = -1;
            }

            return move;
        }

        private void OnCollisionEnter(Collision other)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ON_COLLISION, other.gameObject));
        }

        private void Deceleration(WorldEvent objectEvent)
        {
            _bezier.speed /= 2; //todo откомменить
        }

        private void EnableSpeedBoost(WorldEvent objectEvent)
        {
            _bezier.speed = _maxSpeed;
        }

        private void DisableSpeedBoost(WorldEvent objectEvent)
        {
            // _maxSpeed /= float.Parse(objectEvent.SpeedBooster.Params["SpeedBoost"]);
            // _acceleration /= float.Parse(objectEvent.SpeedBooster.Params["AccelerationBoost"]);
        }
        
    }
}