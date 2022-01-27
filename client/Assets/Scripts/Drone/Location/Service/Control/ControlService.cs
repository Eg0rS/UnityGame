using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.Service.Control.Drone.Event;
using Drone.Location.Service.Game.Event;
using Drone.Location.World;
using IoC.Attribute;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Drone.Location.Service.Control
{
    public class ControlService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private DroneWorld _gameWorld;
        private const float HORISONTAL_SWIPE_ANGLE = 0.40f;
        private const float VERTICAL_SWIPE_ANGLE = 0.70f;

        private const float QUICK_GESTURE_TRESHOLD = 0.10f;
        private const float LONG_TERM_GESTURE_TRESHOLD = 0.20f;
        private const float GESTURE_SWITCH_TIME = 0.5f;

        private Vector2 _beginPosition;
        private Vector2 _currentPosition;
        private float _startTime;
        private bool _isQuickGestureDone;
        private float _width;

        private InputControl _inputControl;

        private bool _isFirstTapDone;

        public void Init()
        {
            _width = Screen.width;
            _gameWorld.AddListener(WorldEvent.CREATED, OnWorldCreated);
            _inputControl = new InputControl();
            TouchSimulation.Enable();
           
        }

        private void OnWorldCreated(GameEvent obj)
        {
            _inputControl.Enable();
            _inputControl.Player.Touch.performed += ctx => OnTouch(ctx.ReadValue<TouchState>());
            _inputControl.Player.Tap.started += ctx => OnTap(ctx);
        }

        
        private void OnDisable()
        {
            _inputControl.Disable();
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            if (_isFirstTapDone) {
                return;
            }
            _isFirstTapDone = true;
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.START_GAME));
        }

        private void OnTouch(TouchState touchState)
        {
            TouchPhase touchPhase = touchState.phase;
            switch (touchPhase) {
                case TouchPhase.Began:
                    _beginPosition = touchState.position;
                    _startTime = Time.time;
                    _isQuickGestureDone = false;
                    break;
                case TouchPhase.Moved:
                    _currentPosition = touchState.position;
                    DefGesture();
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
            Vector2 vector = _currentPosition - _beginPosition;
            float distance = Vector2.Distance(_currentPosition, _beginPosition) / _width;
            if (distance >= QUICK_GESTURE_TRESHOLD && !_isQuickGestureDone) {
                vector = RoundVector(vector);
                _isQuickGestureDone = true;
                _beginPosition = _currentPosition;
                _gameWorld.Dispatch(new ControllEvent(ControllEvent.GESTURE, vector));
            }
        }

        private void LongTermGesture()
        {
            Vector2 vector = _currentPosition - _beginPosition;
            float distance = Vector2.Distance(_currentPosition, _beginPosition) / _width;
            if (distance >= LONG_TERM_GESTURE_TRESHOLD) {
                vector = RoundVector(vector);
                _beginPosition = _currentPosition;
                _gameWorld.Dispatch(new ControllEvent(ControllEvent.GESTURE, vector));
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
                throw new Exception("Vector is not difined");
            }
            return gestureVector;
        }
    }
}