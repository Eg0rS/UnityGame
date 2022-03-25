using System.Linq;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Location.Interface;
using Drone.Location.Model;
using Drone.Location.Model.Player;
using Drone.Location.Service.Game.Event;
using Drone.Location.World.Player.Model;
using Drone.Location.World.Player.Service;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Player
{
    public class PlayerController : GameEventDispatcher, IWorldObjectController<PlayerModel>
    {
        public WorldObjectType ObjectType { get; }
        private const string MESH = "Mesh";
        private const string COLLIDER = "DroneCollider";

        [Inject]
        private DroneWorld _gameWorld;
        [Inject]
        private DroneService _droneService;

        private PlayerTransitionController _transitionController;
        private PlayerAnimatorController _animatorController;

        private GameObject _collider;
        private GameObject _mesh;
        private GameObject _prefab;
        private GameObject _particles;

        public void Init(PlayerModel model)
        {
            gameObject.SetActive(false);
            _prefab = gameObject.GetChildren()[0];
            _particles = _prefab.GetChildren().First(x => x.name == "pfParticleWorld");
            ControllerInitialization();
            SetDroneParameters();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.RESPAWN, OnRespawn);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CUTSCENE_BEGIN, OnBeginCutScene);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CUTSCENE_END, OnEndCutScene);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
        
        }

        private void OnEndGame(InGameEvent obj)
        {
            if (obj.EndGameReason == EndGameReasons.VICTORY) {
                gameObject.SetActive(false);
            }
        }

        private void OnEndCutScene(InGameEvent obj)
        {
            gameObject.SetActive(true);
        }

        private void OnBeginCutScene(InGameEvent obj)
        {
            gameObject.SetActive(false);
        }

        private void OnRespawn(InGameEvent obj)
        {
            Collider[] overLappedColliders = Physics.OverlapSphere(transform.position, 25);
            foreach (Collider collider in overLappedColliders) {
                IInteractiveObject interactiveObject = collider.GetComponent<IInteractiveObject>();
                if (interactiveObject == null) {
                    collider.gameObject.SetActive(false);
                }
            }
            _transitionController.SetDefaultPosition();
        }

        private void ControllerInitialization()
        {
            _collider = _prefab.GetChildren().Find(go => go.name == COLLIDER);
            _mesh = _collider.GetChildren().Find(go => go.name == MESH);
            _animatorController = _mesh.AddComponent<PlayerAnimatorController>();
            _transitionController = _collider.AddComponent<PlayerTransitionController>();
            _animatorController.Configure();
            _animatorController.Particles = _particles;
            _transitionController.Configure(_mesh);
        }

        private void SetDroneParameters()
        {
            DroneModel model = new DroneModel(_droneService.GetDronById(_droneService.SelectedDroneId).DroneDescriptor);
            _transitionController.Mobility = model.mobility;
            CreateDrone(model.DroneDescriptor.Prefab);
        }

        private void CreateDrone(string droneDescriptorPrefab)
        {
            GameObject drone = Instantiate(Resources.Load<GameObject>(droneDescriptorPrefab));
            _gameWorld.AddGameObject(drone, _mesh);
            _animatorController.SetMesh();
        }
    }
}