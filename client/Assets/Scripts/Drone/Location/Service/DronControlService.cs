using AgkCommons.Event;
using Drone.Settings.Service;
using Drone.World.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service
{
    public class DronControlService : GameEventDispatcher
    {
        private const float SWIPE_TRESHOLD = 0.077f;
        private float _width;
        private float _height;

        private Vector2 _currentTouch;
        private Vector2 _startTouch;
        private bool OnSwiping;

        [Inject]
        private SettingsService _settingsService;

        private void Awake()
        {
            _width = Screen.width;
            _height = Screen.height;
            _width *= _height / _width;
            _height *= _height / _width;
        }

        private void Update()
        {
            // if (_settingsService.GetSwipeControl()) {
            //     return;
            // }

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
            float lengthX = Mathf.Abs(swipeVector.x / _width);
            float lengthY = Mathf.Abs(swipeVector.y / _height);

            if (lengthX <= SWIPE_TRESHOLD && lengthY <= SWIPE_TRESHOLD) {
                return;
            }
            _startTouch = _currentTouch;
            
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