﻿using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using DigitalRubyShared;
using Drone.Core.Service;
using Drone.Location.World.Drone.Event;
using Drone.World;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Drone.Location.World.Drone
{
    public class DroneControlService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
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

        public void Init()
        {
            _width = Screen.width;
        }

        private void OnEnable()
        {
            _inputControl.Enable();
        }

        private void Awake()
        {
            Debug.Log(Touchscreen.current);
            _inputControl = new InputControl();
            TouchSimulation.Enable();
            
            //_inputControl.Player.Touch.performed += context =>perf(context);
            
            
            _inputControl.Player.touch1.performed += touch;
            _inputControl.Player.touch2.performed += tval;
            _inputControl.Player.touch3.performed += tany;

        }
        
        private void tany(InputAction.CallbackContext obj)
        {
            Debug.Log("touch any + " + obj.ReadValueAsObject());
        }
        private void tval(InputAction.CallbackContext obj)
        {
            Debug.Log("touch val + " + obj.ReadValueAsObject());
        }

        private void touch(InputAction.CallbackContext obj)
        {
            Debug.Log("touch bool + " + obj.ReadValueAsObject());
            var t =  obj.ReadValue<TouchState>();
            Debug.Log(t);
        }
        

        private void OneTouch(TouchState touchState)
        {
            Debug.Log("process");
            TouchPhase phase = touchState.phase;
            switch (phase) {
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

        private void OnDisable()
        {
            _inputControl.Disable();
        }

        private void Ma()
        {
            Debug.Log("aaaaa");
        }

        // private void OnGesture(TouchControl touch)
        // {
        //     Debug.Log("tousch");
        //     
        //     // switch (touch.phase) {
        //     //     case TouchState:
        //     //         _beginPosition = touch.screenPosition;
        //     //         _startTime = Time.time;
        //     //         _isQuickGestureDone = false;
        //     //         break;
        //     //     case UnityEngine.InputSystem.TouchPhase.Moved:
        //     //         _currentPosition = touch.screenPosition;
        //     //         DefGesture();
        //     //         break;
        //     //     case UnityEngine.InputSystem.TouchPhase.Stationary:
        //     //         Debug.LogWarning("stationary");
        //     //         break;
        //     // }
        // }

        //upate input
        /*  private void Update()
          {
              if (Input.touchCount > 0) {
                  Touch touch = Input.touches[0];
                  OnGesture(touch);
              }
          }
 
         
           private void OnGesture(Touch touch)
         {
             switch (touch.phase) {
                 case TouchPhase.Began:
                     _beginPosition = touch.position;
                     _startTime = Time.time;
                     _isQuickGestureDone = false;
                     break;
                 case TouchPhase.Moved:
                     _currentPosition = touch.position;
                     DefGesture();
                     break;
                 case TouchPhase.Stationary:
                     _beginPosition = touch.position;
                     _startTime = Time.time;
                     _isQuickGestureDone = false;
                     break;
             }
         }*/

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
                _gameWorld.Require().Dispatch(new ControllEvent(ControllEvent.GESTURE, vector));
            }
        }

        private void LongTermGesture()
        {
            Vector2 vector = _currentPosition - _beginPosition;
            float distance = Vector2.Distance(_currentPosition, _beginPosition) / _width;
            if (distance >= LONG_TERM_GESTURE_TRESHOLD) {
                vector = RoundVector(vector);
                _beginPosition = _currentPosition;
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
                throw new Exception("Vector is not difined");
            }
            return gestureVector;
        }
    }
}