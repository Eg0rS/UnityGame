using AgkCommons.Event;
using DeliveryRush.World.Event;
using UnityEngine;

namespace DeliveryRush.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        public Vector2 _swipeVector;
        private Vector2 _currentTouch;
        private Vector2 _startTouch;

        public const float SWIPE_THRESHOLD = 20f;

        private void Update()
        {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    _startTouch = touch.position;
                    _currentTouch = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved) {
                    _currentTouch = touch.position;
                    DetectSwipeVector(_startTouch,_currentTouch);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled ) {
                    _currentTouch = touch.position;
                    DetectSwipeVector(_startTouch,_currentTouch);
                }
            }
        }

        private void DetectSwipeVector(Vector2 StartPoint, Vector2 EndPoint)
        {
            Vector2 swipeVector = EndPoint - StartPoint;
            if (swipeVector.magnitude >= SWIPE_THRESHOLD) {
                _swipeVector = RoundVector(swipeVector);
            }
            Dispatch(new WorldObjectEvent(WorldObjectEvent.SWIPE, _swipeVector));
        }

        private Vector2 RoundVector(Vector2 vector)
        {
            vector.x /= vector.magnitude;
            vector.y /= vector.magnitude;
            if (vector.x >= 0.33 && vector.x < 0.66) {
                vector.x = 0.5f;
            } 
            else if (vector.x < -0.33 && vector.x >= -0.66) {
                vector.x = -0.5f;
            } else {
                vector.x = Mathf.Round(vector.x);
            }
            
            if (vector.y >= 0.33 && vector.y < 0.66) {
                vector.y = 0.5f;
            } 
            else if (vector.y < -0.33 && vector.y >= -0.66) {
                vector.y = -0.5f;
            } else {
                vector.y = Mathf.Round(vector.y);
            }
            
            return vector;
        }
    }
}