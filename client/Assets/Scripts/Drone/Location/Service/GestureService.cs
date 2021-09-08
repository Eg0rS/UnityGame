using AgkCommons.Event;
using Drone.World.Event;
using UnityEngine;

namespace Drone.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        private const float SwipeThreshold = 0.03f;
        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private float _width;
        private float _height;
        private bool OnSwiping = false;

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

            Touch touch = Input.touches[0];
            switch (touch.phase) {
                case TouchPhase.Began:
                    _startTouch = touch.position;
                    _currentTouch = touch.position;
                    break;
                case TouchPhase.Moved when OnSwiping:
                    return;
                case TouchPhase.Moved:
                    _currentTouch = touch.position;
                    DetectSwipe();
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnSwiping = false;
                    _startTouch = Vector2.zero;
                    _currentTouch = Vector2.zero;
                    break;
                case TouchPhase.Stationary:
                    break;
            }
            /*
if UNITY_EDITOR
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _startTouch = Input.mousePosition;
                    _currentTouch = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    _currentTouch = Input.mousePosition;
                    DetectSwipe();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _startTouch = Vector2.zero;
                    _currentTouch = Vector2.zero;
                }
            }
endif
*/
        }

        private void DetectSwipe()
        {
            Vector2 swipeVector = _currentTouch - _startTouch;
            float lengthX = Mathf.Abs(swipeVector.x / _width);
            float lengthY = Mathf.Abs(swipeVector.y / _height);

            if (lengthX <= SwipeThreshold && lengthY <= SwipeThreshold) {
                return;
            }
            OnSwiping = true;
            swipeVector = RoundVector(swipeVector);
            //_startTouch = _currentTouch;
            Dispatch(new WorldEvent(WorldEvent.SWIPE, swipeVector));
            OnSwiping = true;
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