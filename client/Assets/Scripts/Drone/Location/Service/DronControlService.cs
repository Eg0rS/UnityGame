using AgkCommons.Event;
using Drone.Settings.Service;
using Drone.World.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service
{
    public class DronControlService : GameEventDispatcher
    {
        private const float SWIPE_TRESHOLD = 0.005f;
        private const float END_MOVE_TRESHOLD = 0.08f;
        private float _width;
        private float _height;

        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private bool OnSwiping;
        private bool _isMoving = false;
        private Vector2 _movingVector;

        private void Awake()
        {
            _width = Screen.width;
            _height = Screen.height;
            _width *= _height / _width;
            _height *= _height / _width;
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
            
            if (lengthX >= END_MOVE_TRESHOLD || lengthY >= END_MOVE_TRESHOLD) {
                _startTouch = _currentTouch;
                _isMoving = false;
                Dispatch(new WorldEvent(WorldEvent.END_MOVE, currentSwipeVector));
                return;
            }

            if ((lengthX >= SWIPE_TRESHOLD || lengthY >= SWIPE_TRESHOLD) && (!_isMoving || !_movingVector.Equals(currentSwipeVector))) {
                _startTouch = _currentTouch;
                _isMoving = true;
                _movingVector = currentSwipeVector;
                Dispatch(new WorldEvent(WorldEvent.START_MOVE, currentSwipeVector));
                return;
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