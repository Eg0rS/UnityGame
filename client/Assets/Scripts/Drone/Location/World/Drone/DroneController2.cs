using AgkCommons.Event;
using AgkCommons.Extension;
using Cinemachine;
using Drone.Location.Model;
using Drone.Location.Model.Drone;
using Drone.Location.Service.Control;
using Drone.Location.Service.Control.Drone;
using Drone.Location.Service.Game.Event;
using Drone.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneController2 : GameEventDispatcher, IWorldObjectController<DroneModel>
    {
        public WorldObjectType ObjectType { get; }
        private const float CRASH_NOISE = 2;
        private const float CRASH_NOISE_DURATION = 0.5f;
        private const string MESH = "Mesh";
        private const string COLLIDER = "DroneCollider";
        
        [Inject]
        private GameWorld _gameWorld;
        
        private DroneAnimationController _droneAnimationController;
        private DroneTransitionController _droneTransitionController;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        private GameObject _collider;
        private GameObject _mesh;

        private bool _isGameRun;
        
        public void Init(DroneModel model)
        {
            ControllerInitialization();
            _droneAnimationController = gameObject.AddComponent<DroneAnimationController>();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);
        }

        private void ControllerInitialization()
        {
            _collider = gameObject.GetChildren().Find(go => go.name == COLLIDER);
            _mesh = gameObject.GetChildren().Find(go => go.name == MESH);

            _droneTransitionController = _collider.AddComponent<DroneTransitionController>();
        }

        private void OnSetParameters(InGameEvent inGameEvent)
        {
            CreateDrone(inGameEvent.DroneModel.DroneDescriptor.Prefab);
        }
        private void CreateDrone(string droneDescriptorPrefab)
        {
            GameObject drone = Instantiate(Resources.Load<GameObject>(droneDescriptorPrefab));
            _gameWorld.AddGameObject(drone, gameObject.GetChildren().Find(x => x.name == "Mesh"));
        }

        private void OnStartGame(InGameEvent obj)
        {
            _isGameRun = true;
        }
    }
}