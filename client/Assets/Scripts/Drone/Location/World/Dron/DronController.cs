using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using IoC.Attribute;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Dron;
using Drone.Location.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Util;

namespace Drone.Location.World.Dron
{
    public class DronController : GameEventDispatcher, IWorldObjectController<DronModel>
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private GestureService _gestureService;

        public WorldObjectType ObjectType { get; }
        
        private const float AccelerationCoefficient = 0.2f;
        private const float ShiftSpeed = 0.08f;
        private BezierWalkerWithSpeed _bezier;
        private float _levelSpeed = 8;
        private bool _isGameRun;
        private float _boostSpeed;
        private Vector3 _currentPosition;
        private Coroutine _isMoving;

        public void Init(DronModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _gestureService.AddListener<WorldEvent>(WorldEvent.SWIPE, OnSwiped);
            _currentPosition = transform.localPosition;
        }

        private void StartGame(WorldEvent worldEvent)
        {
            _isGameRun = true;
            _bezier.enabled = true;
        }

        public void Update()
        {
            if (!_isGameRun) {
                return;
            }

            if (_bezier.NormalizedT < 0.5f) {
                _levelSpeed += AccelerationCoefficient * Time.deltaTime;
            }
            _bezier.speed = _levelSpeed;
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _gestureService.RemoveListener<WorldEvent>(WorldEvent.SWIPE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.DRON_BOOST_SPEED, Acceleration);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void OnSwiped(WorldEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);
            if (IsPossibleSwipe(swipe)) {
                _currentPosition += swipe;
                MoveTo(_currentPosition);
            }
        }

        private bool IsPossibleSwipe(Vector3 swipe)
        {
            Vector3 newPos = _currentPosition + swipe;
            return (newPos.x <= 1.1f && newPos.x >= -1.1f) && (newPos.y <= 1.1f && newPos.y >= -1.1f);
        }

        private void MoveTo(Vector3 newPos)
        {
            if (_isMoving != null) {
                StopCoroutine(_isMoving);
            }
            _isMoving = StartCoroutine(Moving(newPos));
        }

        private IEnumerator Moving(Vector3 newPos)
        {
            Vector3 prevPos = transform.localPosition;
            float distance = (newPos - prevPos).magnitude;
            float shiftingCoefficient = 0;
            while (shiftingCoefficient < 1) {
                shiftingCoefficient += ShiftSpeed / distance;
                transform.localPosition = Vector3.Lerp(prevPos, newPos, shiftingCoefficient);
                yield return null;
            }
            _isMoving = null;
        }

        private void OnCollisionEnter(Collision other)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ON_COLLISION, other.gameObject));
        }

        private void Acceleration(WorldEvent objectEvent)
        {
            _boostSpeed = objectEvent.SpeedBoost;
            _levelSpeed += _boostSpeed;
            Invoke(nameof(DisableAcceleration), objectEvent.SpeedBoostTime);
        }

        private void DisableAcceleration()
        {
            _levelSpeed -= _boostSpeed;
        }
    }
}