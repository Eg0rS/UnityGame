using AgkCommons.Event;
using DG.Tweening;
using Drone.Location.Service.Control.Drone;
using Drone.Location.Service.Control.Drone.Event;
using Drone.World;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneTransitionController : GameEventDispatcher
    {
        [InjectComponent]
        private Rigidbody _rigidbody;
        private Sequence _sequence;

        private float _mobility;
        private Vector3 _currentPosition = Vector3.zero;
        private DroneAnimationController _animationController;

        public void Configure()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false).SetUpdate(UpdateType.Fixed);
            this.InjectComponents();
        }

        public void OnGesture(ControllEvent controllEvent)
        {
            Vector3 swipe = new Vector3(controllEvent.Gesture.x, controllEvent.Gesture.y, 0f);
            Vector3 newPosition = NewPosition(_currentPosition, swipe);
            if (_currentPosition.Equals(newPosition)) {
                return;
            }
            Move(newPosition);
        }

        private void Move(Vector3 newPosition)
        {
            _currentPosition = newPosition;
            //_sequence.Append(_rigidbody.DOMove(newPosition, _mobility)).SetUpdate(UpdateType.Fixed);
        }

        private Vector3 NewPosition(Vector3 currentPosition, Vector3 swipe)
        {
            // выбрасывание ивента о свайпе в крайнее положение 
            Vector3 newPos = currentPosition + swipe;
            if (newPos.x > 1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.x < -1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.y > 1.0f) {
                swipe.y = 0.0f;
            }
            if (newPos.y < -1.0f) {
                swipe.y = 0.0f;
            }
            Vector3 newPosition = currentPosition + swipe;
            return newPosition;
        }

        public float Mobility
        {
            get { return _mobility; }
            set { _mobility = value; }
        }
        public DroneAnimationController AnimationController
        {
            get { return _animationController; }
            set { _animationController = value; }
        }
    }
}