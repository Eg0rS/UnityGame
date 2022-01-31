using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using Cinemachine;
using Drone.Location.Model;
using Drone.Location.Model.Player;
using Drone.Location.Service.Control.Drone.Event;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class PlayerController : GameEventDispatcher, IWorldObjectController<PlayerModel>
    {
        public WorldObjectType ObjectType { get; }
        private const float CRASH_NOISE = 2;
        private const float CRASH_NOISE_DURATION = 0.5f;
        private const string MESH = "Mesh";
        private const string COLLIDER = "DroneCollider";

        [Inject]
        private DroneWorld _gameWorld;

        private PlayerTransitionController _transitionController;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        private GameObject _collider;
        private GameObject _mesh;
        private GameObject _prefab;

        private Service.Control.Drone.Model.DroneModel _model;

        public void Init(PlayerModel model)
        {
            _prefab = gameObject.GetChildren()[0];
            ControllerInitialization();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _gameWorld.AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CHANGE_SPLINE_SEGMENT, OnChangeSplineSegment);
        }

        private void OnChangeSplineSegment(InGameEvent obj)
        {
            Quaternion targetRotation = Quaternion.LookRotation(obj.BezierSegment.GetTangent(), obj.BezierSegment.GetNormal());
            if (targetRotation != transform.rotation) {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedTime);
            }
        }

        private void OnGesture(ControllEvent obj)
        {
            _transitionController.OnGesture(obj);
        }

        private void OnEndGame(InGameEvent inGameEvent)
        {
            EndGameReasons reason = inGameEvent.EndGameReason;
            switch (reason) {
                case EndGameReasons.OUT_OF_ENERGY:
                    break;
                case EndGameReasons.OUT_OF_DURABILITY:
                    //DroneCrash();
                    break;
                case EndGameReasons.VICTORY:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ControllerInitialization()
        {
            _collider = _prefab.GetChildren().Find(go => go.name == COLLIDER);
            _mesh = _collider.GetChildren().Find(go => go.name == MESH);

            _transitionController = _collider.AddComponent<PlayerTransitionController>();
            _transitionController.Configure();
            //  _animationController = _mesh.AddComponent<DroneAnimationController>();
        }

        private void OnSetParameters(InGameEvent inGameEvent)
        {
            _model = inGameEvent.DroneModel;
            _transitionController.Mobility = _model.mobility;
            CreateDrone(_model.DroneDescriptor.Prefab);
        }

        private void CreateDrone(string droneDescriptorPrefab)
        {
            GameObject drone = Instantiate(Resources.Load<GameObject>(droneDescriptorPrefab));
            _gameWorld.AddGameObject(drone, _mesh);
        }

        private void OnStartGame(InGameEvent obj)
        {
        }
    }
}