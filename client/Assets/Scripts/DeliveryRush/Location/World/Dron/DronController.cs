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
        private BezierWalkerWithSpeed _bezier;
        private float _levelSpeed = 10;
        private const float ACCELERATION = 0.2f;
        private bool _isGameRun = false;
        private float _boostSpeed = 0;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public WorldObjectType ObjectType { get; }

        [Inject]
        private GestureService _gestureService;

        public void Init(DronModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.START_GAME, StartGame);
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.DRON_BOOST_SPEED, Acceleration);
            _gestureService.AddListener<WorldObjectEvent>(WorldObjectEvent.SWIPE, OnSwiped);
        }

        private void StartGame(WorldObjectEvent worldObjectEvent)
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

        private void OnSwiped(WorldObjectEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Swipe.x, objectEvent.Swipe.y, 0f);
            if (IsPossibleSwipe(swipe)) {
                transform.localPosition += swipe;
            }
        }

        private bool IsPossibleSwipe(Vector3 swipe)
        {
            Vector3 newPos = transform.localPosition + swipe;
            Debug.Log(newPos);Debug.Log((newPos.x <= 1.1f && newPos.x >= -1.1f) && (newPos.y <= 1.1f && newPos.y >= -1.1f));
            return (newPos.x <= 1.1f && newPos.x >= -1.1f) && (newPos.y <= 1.1f && newPos.y >= -1.1f);
        }

        private void OnCollisionEnter(Collision other)
        {
            _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.ON_COLLISION, other.gameObject));
        }

        private void Acceleration(WorldObjectEvent objectEvent)
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