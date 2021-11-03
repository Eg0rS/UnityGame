using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Core.Service;
using Drone.Location.World.Drone.Event;
using Drone.World;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Drone.Location.World.Drone
{
    public class DroneControlService : GameEventDispatcher , IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        private const float HORISONTAL_SWIPE_ANGLE = 0.40f;
        private const float VERTICAL_SWIPE_ANGLE = 0.70f;

        private const float QUICK_GESTURE_TRESHOLD = 0.10f;
        private const float LONG_TERM_GESTURE_TRESHOLD = 0.25f;
        private const float GESTURE_SWITCH_TIME = 0.5f;

        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private float _startTime;
        private bool _isMoving;
        private float _width;

        private InputControl _inputControl;

        
        public void Init()
        {
            _width = Screen.width;
           
        }
        private void Awake()
        {
            _inputControl = new InputControl();
            _inputControl.Player.Touch.performed += ctx => Gesture(ctx.ReadValue<TouchState>());
        }

        private void OnEnable()
        {
            _inputControl.Enable();
        }

        private void OnDisable()
        {
            _inputControl.Disable();
        }

        private void Gesture(TouchState touch)
        {
            switch (touch.phase) {
                case TouchPhase.Began:
                    _startPosition = touch.position;
                    _startTime = Time.time;
                    _isMoving = false;
                    break;
                case TouchPhase.Moved:
                    _currentPosition = touch.position;
                    DefGesture();
                    break;
                case TouchPhase.Ended:

                    break;
            }
        }

        private void DefGesture()
        {
            if (Time.time - _startTime >= GESTURE_SWITCH_TIME) {
                LongTermGesture();
            } else {
                QuickGesture();
            }
        }

        private void QuickGesture()
        {
            Vector2 vector = _currentPosition - _startPosition;
            float distance = Vector2.Distance(_currentPosition, _startPosition) / _width;
            if (distance >= QUICK_GESTURE_TRESHOLD && !_isMoving) {
                vector = RoundVector(vector);
                _isMoving = true;
                _startPosition = _currentPosition;
                _gameWorld.Require().Dispatch(new ControllEvent(ControllEvent.GESTURE, vector));
            }
        }

        private void LongTermGesture()
        {
            Vector2 vector = _currentPosition - _startPosition;
            float distance = Vector2.Distance(_currentPosition, _startPosition) / _width;
            if (distance >= LONG_TERM_GESTURE_TRESHOLD) {
                vector = RoundVector(vector);
                _startPosition = _currentPosition;
                _gameWorld.Require().Dispatch(new ControllEvent(ControllEvent.GESTURE, vector));
            }
        }

        private Vector2 RoundVector(Vector2 vector)
        {
            int xSign = Math.Sign(vector.x);
            int ySign = Math.Sign(vector.y);
            Vector2 absVector = vector.Abs();

            float hypotenuse = Vector2.Distance(new Vector2(0, 0), absVector);
            double angle = Math.Sin(absVector.y / hypotenuse);

            Vector2 gestureVector = new Vector2();
            if (angle >= 0.00 && angle <= HORISONTAL_SWIPE_ANGLE) {
                gestureVector.x = 1 * xSign;
                gestureVector.y = 0;
            } else if (angle > HORISONTAL_SWIPE_ANGLE && angle < VERTICAL_SWIPE_ANGLE) {
                gestureVector.x = 1 * xSign;
                gestureVector.y = 1 * ySign;
            } else if (angle >= VERTICAL_SWIPE_ANGLE && angle <= 0.90) {
                gestureVector.x = 0;
                gestureVector.y = 1 * ySign;
            } else {
                throw new Exception("vector not difined");
            }
            return gestureVector;
        }
    }
}