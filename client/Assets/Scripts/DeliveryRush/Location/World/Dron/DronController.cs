using System.Collections;
using UnityEngine;
using AgkCommons.Event;
using IoC.Attribute;
using BezierSolution;
using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.Dron;
using DeliveryRush.Location.Service;
using DeliveryRush.World;
using DeliveryRush.World.Event;
using IoC.Util;

namespace DeliveryRush.Location.World.Dron
{
    public class DronController : GameEventDispatcher, IWorldObjectController<DronModel>
    {
        private const float ACCELERATION = 0.2f;
        private BezierWalkerWithSpeed _bezier;
        private float _levelSpeed = 8;
        private bool _isGameRun;
        private float _boostSpeed;
        private Vector3 _currentPosition;
        private float _shiftSpeed = 0.03f;
        private Coroutine _isMoving;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public WorldObjectType ObjectType { get; }

        [Inject]
        private GestureService _gestureService;

        public void Init(DronModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_GAME, StartGame);
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
                _levelSpeed += ACCELERATION * Time.deltaTime;
            }
            _bezier.speed = _levelSpeed;
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _gestureService.RemoveListener<WorldEvent>(WorldEvent.SWIPE, OnSwiped);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_GAME, StartGame);
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
            float shiftingСoefficient = 0;
            while (shiftingСoefficient < 1) {
                shiftingСoefficient += _shiftSpeed / distance;
                transform.localPosition = Vector3.Lerp(prevPos, newPos, shiftingСoefficient);
                yield return new WaitForSeconds(0.01f);
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