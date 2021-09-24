using System;
using AgkCommons.Event;
using Drone.Location.World.Dron;
using Drone.Settings.Service;
using Drone.World.Event;
using IoC.Attribute;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

namespace Drone.Location.Service
{
    public class DronControlService : GameEventDispatcher
    {
        //
        // private DroneMovement _droneMovement;
        //

        private const float SWIPE_TRESHOLD = 0.05f;
        private const float END_MOVE_TRESHOLD = 0.1f;
        private const float DOUBLE_END_MOVE_TRESHOLD = 0.35f;
        private float _width;
        private float _height;

        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private bool OnSwiping;
        private bool _isMoving;
        private bool _firstSwipeDone;
        private Vector2 _movingVector;

        private Vector2 _swipeVector;

        private void Awake()
        {
            //
            //   _droneMovement = new DroneMovement();
            //
            _width = Screen.width;
            _height = Screen.height;
            // _width *= _height / _width;
            // _height *= _height / _width;
        }

        // private void OnEnable()
        // {
        //     _droneMovement.Enable();
        // }
        //
        // private void OnDisable()
        // {
        //     _droneMovement.Disable();
        // }
        //
        // private void Start()
        // {
        //     _droneMovement.Touch.Contact.started += ctx => StartTouchContact(ctx);
        //     _droneMovement.Touch.Contact.canceled += ctx => EndTouchContact(ctx);
        //
        // }

        // private void EndTouchContact(InputAction.CallbackContext context)
        // {
        //     Debug.Log("contact started " + _droneMovement.Touch.Position.ReadValue<Vector2>());
        // }
        //
        // private void StartTouchContact(InputAction.CallbackContext context)
        // {
        //     Debug.Log("contact ended " + _droneMovement.Touch.Position.ReadValue<Vector2>());
        // }

        private void Update()
        {
            if (Input.touchCount <= 0) {
                return;
            }

            Touch touch = Input.touches[Input.touchCount - 1];
            switch (touch.phase) {
                case TouchPhase.Began:
                    _startTouch = touch.position;
                    _currentTouch = touch.position;
                    break;
                case TouchPhase.Moved:
                    _currentTouch = touch.position;
                    DetectSwipe();
                    OnSwiping = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnSwiping = false;
                    Dispatch(new WorldEvent(WorldEvent.SWIPE_END)); // swipe ended
                    break;
                case TouchPhase.Stationary:
                    break;
            }
        }

        private void DetectSwipe()
        {
            Vector2 currentSwipeVector = _currentTouch - _startTouch;
            float lengthX = Mathf.Abs(currentSwipeVector.x / _width);
            float lengthY = Mathf.Abs(currentSwipeVector.y / _height);

            if (lengthX <= SWIPE_TRESHOLD && lengthY <= SWIPE_TRESHOLD) {
                return;
            }

            currentSwipeVector = RoundVector(currentSwipeVector);

            bool vectorChanged = !_movingVector.Equals(currentSwipeVector);
            if (vectorChanged) {
                _firstSwipeDone = false;
                _swipeVector = Vector2.zero;
                _isMoving = false;
                _movingVector = currentSwipeVector;
            }
            
            if ((lengthX >= SWIPE_TRESHOLD || lengthY >= SWIPE_TRESHOLD) && !_isMoving) {
                _startTouch = _currentTouch;
                _isMoving = true;
               
                Dispatch(new WorldEvent(WorldEvent.END_MOVE, currentSwipeVector));
                Debug.Log("Start move: " + currentSwipeVector);
                return;
            }

            if (!_firstSwipeDone) {
                if (lengthX >= END_MOVE_TRESHOLD || lengthY >= END_MOVE_TRESHOLD) {
                    _startTouch = _currentTouch;
                    _firstSwipeDone = true;
                    _swipeVector = currentSwipeVector;
                    _movingVector = currentSwipeVector;
                    Dispatch(new WorldEvent(WorldEvent.END_MOVE, currentSwipeVector));
                    Debug.Log("Swape: " + currentSwipeVector);
                }

                return;
            }

            if (currentSwipeVector.Equals(_swipeVector) && lengthX >= DOUBLE_END_MOVE_TRESHOLD || lengthY >= DOUBLE_END_MOVE_TRESHOLD) {
                _startTouch = _currentTouch;
                Dispatch(new WorldEvent(WorldEvent.END_MOVE, currentSwipeVector));
                Debug.Log("Double swape: " + currentSwipeVector);
            }
        }

        private Vector2 RoundVector(Vector2 vector)
        {
            vector = vector.normalized;

            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);

            return vector;
        }
    }
}