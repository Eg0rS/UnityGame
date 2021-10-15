using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Location.World.Drone.Event;
using UnityEngine;
using UnityEngine.Serialization;
using TouchPhase = UnityEngine.TouchPhase;

namespace Drone.Location.World.Drone
{
    public class DroneControlService : GameEventDispatcher
    {
        #region const

        [FormerlySerializedAs("SWIPE_TRESHOLD")]
        [Header("default value = 0.05")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _swipeTreshold = 0.03f;

        [FormerlySerializedAs("END_MOVE_TRESHOLD")]
        [Header("default value = 0.1")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _endMoveTreshold = 0.07f;

        [FormerlySerializedAs("DOUBLE_END_MOVE_TRESHOLD")]
        [Header("default value = 0.35")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _doubleEndMoveTreshold = 0.95f;

        [FormerlySerializedAs("HORISONTAL_SWIPE_ANGLE")]
        [Header("default value = 0.30")]
        [Range(0.0f, 0.90f)]
        [SerializeField]
        private double _horisontalSwipeAngle = 0.30;
        
        [FormerlySerializedAs("VERTICAL_SWIPE_ANGLE")]
        [Header("default value = 0.70")]
        [Range(0.0f, .90f)]
        [SerializeField]
        private double _verticalSwipeAngle = 0.7f;


        #endregion

        private float _width;
        private float _height;

        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private bool _isMoving;
        private bool _swipeDone;
        private Vector2 _movingVector;

        private Vector2 _swipeVector;

        private void Awake()
        {
            _width = Screen.width;
            _height = Screen.height;
        }

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
                    break;
                case TouchPhase.Ended:
                    _swipeDone = true;
                    break;
                case TouchPhase.Canceled:
                    _swipeDone = true;
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

            if (lengthX <= _swipeTreshold && lengthY <= _swipeTreshold) {
                return;
            }

            currentSwipeVector = RoundVector(currentSwipeVector);

            bool vectorChanged = !_movingVector.Equals(currentSwipeVector);
            if (vectorChanged) {
                _swipeDone = false;
                _swipeVector = Vector2.zero;
                _isMoving = false;
                _movingVector = currentSwipeVector;
            }

            if ((lengthX >= _swipeTreshold || lengthY >= _swipeTreshold) && !_isMoving) {
                _startTouch = _currentTouch;
                _isMoving = true;

                Dispatch(new ControllEvent(ControllEvent.START_MOVE, currentSwipeVector));
                Debug.Log("Start move: " + currentSwipeVector);
                return;
            }

            if (_swipeDone) {
                if (lengthX >= _endMoveTreshold || lengthY >= _endMoveTreshold) {
                    _startTouch = _currentTouch;
                    _isMoving = false;
                    _swipeVector = currentSwipeVector;
                    _movingVector = currentSwipeVector;
                    Dispatch(new ControllEvent(ControllEvent.END_MOVE, currentSwipeVector));
                    Debug.Log("Swape: " + currentSwipeVector);
                }
            }
        }

        private Vector2 RoundVector(Vector2 vector)
        {
            vector = vector.normalized;

            int xSign = Math.Sign(vector.x);
            int ySign = Math.Sign(vector.y);
            Vector2 absVector = vector.Abs();

            float hypotenuse = Vector2.Distance(new Vector2(0, 0), absVector);

            double angle = Math.Sin(absVector.y / hypotenuse);
            Vector2 swipeVector = new Vector2();
            if (angle > 0.00 && angle <= _horisontalSwipeAngle) {
                swipeVector.x = 1 * xSign;
                swipeVector.y = 0;
            } else if (angle > _horisontalSwipeAngle && angle <= _verticalSwipeAngle) {
                swipeVector.x = 1 * xSign;
                swipeVector.y = 1 * ySign;
                
            } else if (angle > _verticalSwipeAngle && angle <= 0.90) {
                swipeVector.x = 0;
                swipeVector.y = 1 * ySign;
            }
            
            return swipeVector;
        }
    }
}