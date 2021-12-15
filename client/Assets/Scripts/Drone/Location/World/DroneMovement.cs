using DG.Tweening;
using Drone.Location.Service.Control.Drone.Event;
using Drone.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service.Control
{
    public class DroneMovement : MonoBehaviour
    {
        [Inject]
        private GameWorld _gameWorld;

        private float _mobility;
        private float _baseMobility;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private Rigidbody _rigidbody;
        private Sequence _sequence;

        private const float MINIMAL_SPEED = 3.0f;

        private void Awake()
        {
            _gameWorld.AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _rigidbody = GetComponentsInChildren<Rigidbody>()[0];
        }

        private void OnGesture(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Gesture.x, objectEvent.Gesture.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            _gameWorld.Dispatch(new ControllEvent(ControllEvent.MOVEMENT, newPosition));
            DotWeenMove(newPosition);
        }

        private Vector3 NewPosition(Vector3 dronPos, Vector3 swipe)
        {
            Vector3 newPos = dronPos + swipe;
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
            Vector3 newPosition = dronPos + swipe;
            return newPosition;
        }

        private void DotWeenMove(Vector3 newPos)
        {
            _mobility = _baseMobility * (MINIMAL_SPEED / 10);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.localRotation.y, _droneTargetPosition.x - newPos.x) * 30;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility).SetUpdate(UpdateType.Fixed))
                     .Join(transform.DOLocalRotate(rotation, _mobility)
                                    .SetUpdate(UpdateType.Fixed)
                                    .OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility).SetUpdate(UpdateType.Fixed); }));
        }
    }
}