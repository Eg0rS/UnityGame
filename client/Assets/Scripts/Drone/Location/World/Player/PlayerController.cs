using System.Linq;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Location.Interface;
using Drone.Location.Model;
using Drone.Location.Model.Player;
using Drone.Location.Service.Game.Event;
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

        private PlayerTransitionController _transitionController;
        private PlayerAnimatorController _animatorController;

        private GameObject _collider;
        private GameObject _mesh;
        private GameObject _prefab;
        private GameObject _particles;

        private Service.Control.Drone.Model.DroneModel _model;

        public void Init(PlayerModel model)
        {
            _prefab = gameObject.GetChildren()[0];
            _particles = _prefab.GetChildren().First(x => x.name == "pfParticleWorld");
            ControllerInitialization();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.CHANGE_SPLINE_SEGMENT, OnChangeSplineSegment);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.RESPAWN, OnRespawn);
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
            _transitionController.SetCurrentPosition();
        }

        private void OnChangeSplineSegment(InGameEvent obj)
        {
            Quaternion targetRotation = Quaternion.LookRotation(obj.BezierSegment.GetTangent(), obj.BezierSegment.GetNormal());
            if (targetRotation != transform.rotation) {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedTime);
            }
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
            _animatorController.SetMesh();
        }
    }
}