using System.Collections;
using AgkCommons.Event;
using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Drone;
using Drone.Location.World.Drone.Event;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using DG.Tweening;

namespace Drone.Location.World.Drone
{
    public class DroneController : GameEventDispatcher, IWorldObjectController<DronePrefabModel>
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private DroneAnimationController _droneAnimationController;

        private float _acceleration;
        private float _maxSpeed;
        private float _basemobility;
        private float _mobility;
        private BezierWalkerWithSpeed _bezier;
        private Vector3 _droneTargetPosition = Vector3.zero;
        private const float MINIMAL_SPEED = 3.0f;
        private bool _isGameRun;
        public WorldObjectType ObjectType { get; }

        private Sequence _sequence;

        public void Init(DronePrefabModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            _droneAnimationController = gameObject.AddComponent<DroneAnimationController>();
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SPEED, EnableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SPEED, DisableSpeedBoost);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, SetParameters);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, Deceleration);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.PLAY_ANIMATE, OnPlayAnimation);

            _sequence = DOTween.Sequence();
        }

        private void OnPlayAnimation(WorldEvent obj)
        {
            _droneAnimationController.OnPlayAnimation(obj);
        }
        private void SetParameters(WorldEvent worldEvent)
        {
            _bezier.enabled = false;
            _bezier.speed = MINIMAL_SPEED;
            _maxSpeed = worldEvent.DroneModel.maxSpeed;
            _acceleration = worldEvent.DroneModel.acceleration;
            _basemobility = worldEvent.DroneModel.mobility;
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
            SetBezierSpeed();
        }

        private void SetBezierSpeed()
        {
            if (_bezier.speed >= _maxSpeed) {
                return;
            }
            float newSpeed = _bezier.speed + _acceleration * Time.deltaTime;
            _bezier.speed = newSpeed > _maxSpeed ? _maxSpeed : newSpeed;
        }

        private void EndGame(WorldEvent worldEvent)
        {
            _gameWorld.Require().RemoveListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.ENABLE_SPEED, EnableSpeedBoost);
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.END_GAME, EndGame);
        }

        private void OnGesture(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Gesture.x, objectEvent.Gesture.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
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
            _mobility = _basemobility * (MINIMAL_SPEED / _bezier.speed);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.rotation.y, _droneTargetPosition.x - newPos.x) * 45;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility));
            //.Join(transform.DOLocalRotate(rotation, _mobility).OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility); }));
        }

        private void Deceleration(WorldEvent objectEvent)
        {
            _bezier.speed /= 2;
            // _bezier.speed = 0;
        }

        private void EnableSpeedBoost(WorldEvent objectEvent)
        {
            _bezier.speed = _maxSpeed;
        }

        private void DisableSpeedBoost(WorldEvent objectEvent)
        {
            // _maxSpeed /= float.Parse(objectEvent.SpeedBooster.Params["SpeedBoost"]);
            // _acceleration /= float.Parse(objectEvent.SpeedBooster.Params["AccelerationBoost"]);
        }
    }
}