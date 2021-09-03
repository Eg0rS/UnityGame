using AgkCommons.Event;
using Drone.World.Event;
using UnityEngine;

namespace Drone.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        private const float SWIPE_THRESHOLD = 0.1f;
        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private float _width;
        private float _height;

        private void Awake()
        {
            _width = Screen.width;
            _height = Screen.height;
            _width *= _height / _width;
            _height *= _height / _width;
        }

        private void Update()
        {
            if (Input.touchCount > 0) {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began) {
                    _startTouch = touch.position;
                    _currentTouch = touch.position;
                } else if (touch.phase == TouchPhase.Moved) {
                    _currentTouch = touch.position;
                    DetectSwipe();
                } else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    _startTouch = Vector2.zero;
                    _currentTouch = Vector2.zero;
                }
            }
#if UNITY_EDITOR
            else {
                if (Input.GetMouseButtonDown(0)) {
                    _startTouch = Input.mousePosition;
                    _currentTouch = Input.mousePosition;
                } else if (Input.GetMouseButton(0)) {
                    _currentTouch = Input.mousePosition;
                    DetectSwipe();
                } else if (Input.GetMouseButtonUp(0)) {
                    _startTouch = Vector2.zero;
                    _currentTouch = Vector2.zero;
                }
            }
#endif
        }

        private void DetectSwipe()
        {
            Vector2 swipeVector = _currentTouch - _startTouch;
            float lengthX = Mathf.Abs(swipeVector.x / _width);
            float lengthY = Mathf.Abs(swipeVector.y / _height);

            if (!(lengthX >= SWIPE_THRESHOLD) && !(lengthY >= SWIPE_THRESHOLD)) {
                return;
            }
            swipeVector = RoundVector(swipeVector);
            _startTouch = _currentTouch;
            Dispatch(new WorldEvent(WorldEvent.SWIPE, swipeVector));
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