using AgkCommons.Event;
using DG.Tweening;
using Drone.Location.Service.Control.Drone.Event;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class PlayerTransitionController : GameEventDispatcher
    {
        [Inject]
        private DroneWorld _gameWorld;
        [InjectComponent]
        private Rigidbody _rigidbody;

        private float _mobility;
        private Vector3 _currentPosition = Vector3.zero;

        public void Configure()
        {
            this.InjectComponents();
            this.Inject();
            DOTween.defaultUpdateType = UpdateType.Fixed;
            Vector3 newPosition = new Vector3(0,1,0);
            Vector3[] path = {newPosition};
            transform.DOLocalPath(path, _mobility);
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
            transform.DOLocalMove(newPosition, _mobility);
            // _rigidbody.DOLocalPath(path, _mobility);
        }

        private Vector3 NewPosition(Vector3 currentPosition, Vector3 swipe)
        {
            Vector3 newPos = currentPosition + swipe;
           // _gameWorld.Dispatch(new ControllEvent(ControllEvent.MOVEMENT, newPos));
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