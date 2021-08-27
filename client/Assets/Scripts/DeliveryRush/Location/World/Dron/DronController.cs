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
        private float _speedShift = 1f;
        private Vector2 _currentPos;
        private Vector2 _newPos;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public WorldObjectType ObjectType { get; }

        private GestureService _gestureService;

        public void Init(DronModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _bezier.enabled = false;
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.START_GAME, StartGame);
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.DRON_BOOST_SPEED, Acceleration);
            _gestureService = gameObject.AddComponent<GestureService>();
        }

        private void StartGame(WorldObjectEvent worldObjectEvent)
        {
            _isGameRun = true;
            _bezier.enabled = true;
        }

        public void Update()
        {
            if (!_isGameRun) return;

            if (_bezier.NormalizedT < 0.5f) {
                _levelSpeed += ACCELERATION * Time.deltaTime;
            }
            _bezier.speed = _levelSpeed;
            if (CheckPossibilitySwipe(_gestureService.SwipeVector)) {
                transform.position += (Vector3) _gestureService.SwipeVector;
            } else {
                Debug.Log("Hui" );
            }
        }

        private bool CheckPossibilitySwipe(Vector2 swipe)
        {
            Vector2 Max = new Vector2(1, 1);
            Vector2 Min = new Vector2(-1, -1);
            Vector2 NewPos = (Vector2)transform.localPosition + swipe;
            Debug.Log( transform.localPosition);
            return NewPos.x <= Max.x && NewPos.y <= Max.y && NewPos.x >= Min.x && NewPos.y >= Min.y;
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