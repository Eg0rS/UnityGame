using AgkCommons.Event;
using Drone.World.Event;
using UnityEngine;

namespace Drone.Location.Service
{
    public class GestureService : GameEventDispatcher
    {
        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private bool OnSwiping;

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
                case TouchPhase.Moved when OnSwiping:
                    return;
                case TouchPhase.Moved:
                    _currentTouch = touch.position;
                    DetectSwipe();
                    OnSwiping = true;
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
        }

        private void DetectSwipe()
        {
            Vector2 swipeVector = _currentTouch - _startTouch;
            swipeVector = RoundVector(swipeVector);
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