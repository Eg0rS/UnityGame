using AgkCommons.Event;
using DG.Tweening;
using Drone.Location.Service.Control.Drone.Event;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;

namespace Drone.Location.World.Player
{
    public class PlayerTransitionController : GameEventDispatcher
    {
        [Inject]
        private DroneWorld _gameWorld;
        [InjectComponent]
        private Rigidbody _rigidbody;
        private Transform _mashTransform;

        private float _mobility;
        private Vector3 _currentPosition = Vector3.zero;
        

        public void Configure(GameObject mesh)
        {
            _mashTransform = mesh.transform;
            this.InjectComponents();
            this.Inject();
            _gameWorld.AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
        }

        private void OnGesture(ControllEvent controllEvent)
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
            Vector3 rotation = new Vector3(_currentPosition.y - newPosition.y, transform.localRotation.y, _currentPosition.x - newPosition.x) * 30;
            _currentPosition = newPosition;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.DOLocalPath(new[] {newPosition}, _mobility, PathType.Linear, PathMode.Full3D, 1, Color.green)
                      .SetEase(Ease.OutQuad)
                      .SetUpdate(UpdateType.Fixed)
                      .OnStart(() => _mashTransform.DOLocalRotate(rotation, _mobility)
                                                   .SetUpdate(UpdateType.Fixed)
                                                   .OnComplete(() => _mashTransform.DOLocalRotate(Vector3.zero, _mobility)
                                                                                   .SetUpdate(UpdateType.Fixed)));
        }

        private Vector3 NewPosition(Vector3 currentPosition, Vector3 swipe)
        {
            Vector3 newPos = currentPosition + swipe;
            _gameWorld.Dispatch(new ControllEvent(ControllEvent.MOVEMENT, newPos));
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