﻿using System;
using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using IoC.Attribute;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Drone;
using Drone.Location.Service;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Util;

namespace Drone.Location.World.Dron
{
    public class DronController : GameEventDispatcher, IWorldObjectController<DroneModel>
    {
        private const float UPDATE_TIME = 0.1f;
        private const float AccelerationCoefficient = 0.2f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private DronService _droneService;

        [Inject]
        private GameService _gameService;

        [Inject]
        private DronControlService _dronControlService;

        private Animator _animator;

        public WorldObjectType ObjectType { get; }
        private BezierWalkerWithSpeed _bezier;
        private Coroutine _isMoving;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private float _mobility;
        private float _boostSpeed;
        private float _levelSpeed = 8;
        private bool _isGameRun;

        public void Init(DroneModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.START_MOVE, OnStart);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.END_MOVE, OnSwiped);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.SWIPE_END, OnSwipedEnd);
            DroneDescriptor droneDescriptor = _droneService.GetDroneById(_gameService.DroneId).DroneDescriptor;
            model.SetDroneParameters(droneDescriptor.Mobility, droneDescriptor.Durability, droneDescriptor.Energy);
            _mobility = model.Mobility;

            _animator = transform.GetComponentInParent<Animator>();
        }

        private void StartGame(WorldEvent worldEvent)
        {
            _isGameRun = true;
            _bezier.enabled = true;
            _bezier.speed = _levelSpeed;
        }

        public void Update()
        {
            if (!_isGameRun) {
                return;
            }

            // if (_bezier.NormalizedT < 0.5f) {
            //     _levelSpeed += AccelerationCoefficient * Time.deltaTime;
            // }
            // _bezier.speed = _levelSpeed;
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.START_MOVE, OnStart);
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.END_MOVE, OnSwiped);
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.SWIPE_END, OnSwipedEnd);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void OnSwipedEnd(WorldEvent obj)
        {
            if (!transform.localPosition.Equals(_droneTargetPosition)) {
                MoveTo(_droneTargetPosition);
            }
        }

        private void OnStart(WorldEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);

            if (IsPossibleSwipe(swipe)) {
                Vector3 newPosition = _droneTargetPosition + swipe;
                MoveTo(newPosition);
            }
        }

        private void OnSwiped(WorldEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);

            if (IsPossibleSwipe(swipe)) {
                Vector3 newPosition = _droneTargetPosition + swipe;
                _droneTargetPosition = newPosition;
                MoveTo(newPosition);
            }
        }

        private bool IsPossibleSwipe(Vector3 swipe)
        {
            Vector3 newPos = _droneTargetPosition + swipe;
            return (newPos.x <= 1.1f && newPos.x >= -1.1f) && (newPos.y <= 1.1f && newPos.y >= -1.1f);
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
            
            // if (_animator.GetInteger("moveDirection") != GetMoveDirection(new Vector2(targetPosition.x, targetPosition.y))) {
            //     _animator.SetInteger("moveDirection", GetMoveDirection(new Vector2(targetPosition.x, targetPosition.y)));
            //     _animator.SetFloat("x", targetPosition.x);
            //     _animator.SetFloat("y", targetPosition.y);
            // }
            
            _animator.SetInteger("moveDirection", GetMoveDirection(new Vector2(targetPosition.x, targetPosition.y)));
            _animator.SetFloat("x", targetPosition.x);
            _animator.SetFloat("y", targetPosition.y);
            
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

                transform.localPosition = new Vector3(xPos, yPos, currentPosition.z);

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

        private void Acceleration(WorldEvent objectEvent)
        {
            _boostSpeed = objectEvent.SpeedBoost;
            _levelSpeed += _boostSpeed;
            Invoke(nameof(DisableAcceleration), objectEvent.SpeedBoostTime);
        }

        private void DisableAcceleration()
        {
            _levelSpeed -= _boostSpeed;
        }

        private void RotateSelf(Swipe swipe, float angleRotate)
        {
            if (swipe.Check(HorizontalSwipeDirection.LEFT)) {
                transform.localRotation = Quaternion.Euler(0, 0, angleRotate);
            } else if (swipe.Check(HorizontalSwipeDirection.RIGHT)) {
                transform.localRotation = Quaternion.Euler(0, 0, -angleRotate);
            } else if (swipe.Check(VerticalSwipeDirection.DOWN)) {
                transform.localRotation = Quaternion.Euler(angleRotate * 0.5f, 0, 0);
            } else if (swipe.Check(VerticalSwipeDirection.UP)) {
                transform.localRotation = Quaternion.Euler(-angleRotate * 0.5f, 0, 0);
            }
        }

        private int GetMoveDirection(Vector2 position)
        {
            if (position == Vector2.right) {
                return 2;
            }
            if (position == Vector2.up) {
                return 1;
            }
            if (position == Vector2.left) {
                return 4;
            }
            if (position == Vector2.down) {
                return 3;
            }
            if (position == new Vector2(-1, 1)) {
                return 5;
            }
            if (position == new Vector2(1, 1)) {
                return 6;
            }
            if (position == new Vector2(-1, -1)) {
                return 7;
            }
            if (position == new Vector2(1, -1)) {
                return 8;
            }
            return 0;
        }
    }
}