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
        
        [Inject]
        private SettingsService _settingsService;

        public WorldObjectType ObjectType { get; }
        
        private const float AccelerationCoefficient = 0.2f;
        private float ShiftSpeed = 0.13f;
        private BezierWalkerWithSpeed _bezier;
        private float _levelSpeed = 8;
        private bool _isGameRun;
        private float _boostSpeed;
        private Vector3 _currentPosition;
        private Coroutine _isMoving;
        private float _shiftCoeficient=0;

        public void Init(DronModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _dronControlService.AddListener<WorldEvent>(WorldEvent.SWIPE, OnSwiped);
            _currentPosition = transform.localPosition;

            // ShiftSpeed = model.SpeedShift; // !_settingsService.GetSwipeControl() ? 0.075f : 0.13f; 
            ShiftSpeed = 0.2f; // !_settingsService.GetSwipeControl() ? 0.075f : 0.13f; 
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

            if (_bezier.NormalizedT < 0.5f) {
                _levelSpeed += AccelerationCoefficient * Time.deltaTime;
            }
            _bezier.speed = _levelSpeed;
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _dronControlService.RemoveListener<WorldEvent>(WorldEvent.SWIPE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void OnSwiped(WorldEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);
            if (IsPossibleSwipe(swipe)) {
                _currentPosition += swipe;
                MoveTo(_currentPosition);
            }
        }

        private bool IsPossibleSwipe(Vector3 swipe)
        {
            Vector3 newPos = _currentPosition + swipe;
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
            Vector3 startPosition = transform.localPosition;
            Vector3 move = targetPosition - startPosition;
            float distance = (move).magnitude;
            float time = distance / ShiftSpeed;
            float updateCount = (float) Math.Ceiling(time / UPDATE_TIME);
            float deltaX = move.x / updateCount;
            float deltaY = move.y / updateCount;
            bool complete = false;
            // float delta = 0;
            
            bool upDirection = targetPosition.y > startPosition.y;
            bool rightDirection = targetPosition.x > startPosition.x;
            _shiftCoeficient += ShiftSpeed * UPDATE_TIME;
            
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
                
                if (currentPosition == targetPosition) {
                    complete = true;
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
            if (swipe.Check(HorizontalSwipeDirection.LEFT))
            {
                transform.localRotation = Quaternion.Euler(0,0, angleRotate);
            }
            else if (swipe.Check(HorizontalSwipeDirection.RIGHT))
            {
                transform.localRotation = Quaternion.Euler(0,0, -angleRotate);
            }
            else if (swipe.Check(VerticalSwipeDirection.DOWN))
            {
                transform.localRotation = Quaternion.Euler(angleRotate * 0.5f, 0,0);
            }
            else if (swipe.Check(VerticalSwipeDirection.UP))
            {
                transform.localRotation = Quaternion.Euler( -angleRotate * 0.5f, 0,0);
            }
        }
    }
}