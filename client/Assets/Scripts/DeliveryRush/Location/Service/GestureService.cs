using AgkCommons.Event;
using DeliveryRush.World.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace DeliveryRush.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private float _width = 0;
        private float _height = 0;
        public const float SWIPE_THRESHOLD = 0.01f;

        private void Awake()
        {
            _width = (float) Screen.width / 2.0f;
            _height = (float) Screen.height / 2.0f;
        }

        private void Update()
        {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    _startTouch = touch.position;
                    _currentTouch = touch.position;
                } else if (touch.phase == TouchPhase.Moved) {
                    _currentTouch = touch.position;
                    DetectSwipeVector();
                } else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    _startTouch = Vector2.zero;
                    _currentTouch = Vector2.zero;
                }
            }
        }

        private void DetectSwipeVector()
        {
            Vector2 swipeVector = _currentTouch - _startTouch;
            float lengthX = swipeVector.x / _width;
            float lengthY = swipeVector.y / _height;
            
            if (lengthX >= SWIPE_THRESHOLD || lengthY >= SWIPE_THRESHOLD) {
                swipeVector = RoundVector(swipeVector);
                _startTouch = _currentTouch;
                Dispatch(new WorldObjectEvent(WorldObjectEvent.SWIPE, swipeVector));
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