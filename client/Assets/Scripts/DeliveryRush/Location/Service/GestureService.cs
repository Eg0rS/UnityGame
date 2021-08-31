using AgkCommons.Event;
using DeliveryRush.World.Event;
using UnityEngine;

namespace DeliveryRush.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private float _width = 0;
        private float _height = 0;
        public const float SWIPE_THRESHOLDY = 0.2f;
        public const float SWIPE_THRESHOLDX = 0.3f;

        private void Awake()
        {
            _width = (float) Screen.width / 2.0f;
            _height = (float) Screen.height / 2.0f;
        }

        private void Update()
        {
            if (Input.touchCount <= 0) {
                return;
            }
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

        private void DetectSwipe()
        {
            Vector2 swipeVector = _currentTouch - _startTouch;
            float lengthX = Mathf.Abs(swipeVector.x / _width);
            float lengthY = Mathf.Abs(swipeVector.y / _height);

            if (lengthX >= SWIPE_THRESHOLDX || lengthY >= SWIPE_THRESHOLDY) {
                swipeVector = RoundVector(swipeVector);
                _startTouch = _currentTouch;
                Dispatch(new WorldEvent(WorldEvent.SWIPE, swipeVector));
            }
        }

        private Vector2 RoundVector(Vector2 vector)
        {
            vector = vector.normalized;
            Debug.Log(vector);
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);

            return vector;
        }
    }
}