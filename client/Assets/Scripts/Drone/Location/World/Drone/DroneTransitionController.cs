using AgkCommons.Event;
using DG.Tweening;
using Drone.Location.Service.Control.Drone.Event;
using Drone.World;
using GameKit.World;
using IoC.Attribute;
using IoC.Extension;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneTransitionController : GameEventDispatcher
    {
        [InjectComponent]
        private Rigidbody _rigidbody;
        private Sequence _sequence;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        private float _mobility;
        private Vector3 _currentPosition = Vector3.zero;

        public void Configure()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            this.InjectComponents();
            this.Inject();
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
            Vector3[] path = {newPosition};
            _rigidbody.DOLocalPath(path, _mobility).SetUpdate(UpdateType.Fixed);
        }

        private Vector3 NewPosition(Vector3 currentPosition, Vector3 swipe)
        {
            Vector3 newPos = currentPosition + swipe;
            _gameWorld.Require().Dispatch(new ControllEvent(ControllEvent.MOVEMENT, newPos));
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
    }
}