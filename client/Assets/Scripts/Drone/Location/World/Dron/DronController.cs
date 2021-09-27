using System;
using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using IoC.Attribute;
using BezierSolution;
using Cinemachine.Utility;
using Drone.Location.Model;
using Drone.Location.Model.Dron;
using Drone.Location.Service;
using Drone.Settings.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Util;

namespace Drone.Location.World.Dron
{
    public class DronController : GameEventDispatcher, IWorldObjectController<DronModel>
    {
        private const float UPDATE_TIME = 0.1f;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private DronControlService _dronControlService;

        public WorldObjectType ObjectType { get; }

        private float Acceleration = 0.1f; // прибавление в секунду
        private float MaxSpeed = 20; 
        private float ShiftSpeed = 0.4f;
        private BezierWalkerWithSpeed _bezier;
        private bool _isGameRun;
        private float _boostSpeed;
        private Coroutine _isMoving;
        private Vector3 _droneTargetPosition = Vector3.zero;

        public void Init(DronModel model)
        {
            
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _bezier.speed = 0;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, BoostSpeed);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.START_MOVE, OnStart);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.END_MOVE, OnSwiped);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.SWIPE_END, OnSwipedEnd);

            // ShiftSpeed = model.SpeedShift; // !_settingsService.GetSwipeControl() ? 0.075f : 0.13f; 
            // ShiftSpeed = 0.2f; // !_settingsService.GetSwipeControl() ? 0.075f : 0.13f; 
            ShiftSpeed = 0.7f;
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

            if (_bezier.speed < MaxSpeed) { 
                _bezier.speed += Acceleration * Time.deltaTime;
            }
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.START_MOVE, OnStart);
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.END_MOVE, OnSwiped);
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.SWIPE_END, OnSwipedEnd);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, BoostSpeed);
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
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            Debug.Log(_droneTargetPosition);
            MoveTo(newPosition);
        }

        private void OnSwiped(WorldEvent objectEvent)
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
            float time = distance / ShiftSpeed;
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

        private void Deceleration()
        {
            _bezier.speed /= 2;
        }

        private void BoostSpeed(WorldEvent objectEvent)
        {
            _boostSpeed = objectEvent.SpeedBoost;
            _bezier.speed += _boostSpeed;
            Invoke(nameof(DisableAcceleration), objectEvent.SpeedBoostTime);
        }

        private void DisableAcceleration()
        {
            _bezier.speed -= _boostSpeed;
        }
    }
}