using BezierSolution;
using Drone.Location.Model;
using Drone.Location.Model.Spline;
using UnityEngine;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;

namespace Drone.Location.World.Spline
{
    public class SplineWalkerController : MonoBehaviour, IWorldObjectController<SplineWalkerModel>
    {
        public WorldObjectType ObjectType { get; }
        private const float SPEED = 5f;

        [Inject]
        private DroneWorld _gameWorld;

        private Rigidbody _levelRigidBody;
        private SplineController _splineController;

        public float _distanceTraveled = 0f;
        private bool _isCanFly = false;
        private Vector3 _startPosition;
        private Vector3 _curentPosition = Vector3.zero;
        private const float OVERCLOCKING_DISTANCE = 75f;

        public void Init(SplineWalkerModel model)
        {
            ConfigureRigidBody();
            _splineController = _gameWorld.RequireObjectComponent<SplineController>();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.RESPAWN, OnStartGame);
        }

        private void OnEndGame(InGameEvent obj)
        {
            _isCanFly = false;
        }

        private void OnStartGame(InGameEvent obj)
        {
            _startPosition = _curentPosition;
            _isCanFly = true;
            _levelRigidBody.position = new Vector3(0, 0, _levelRigidBody.position.z);
        }

        private void ConfigureRigidBody()
        {
            _levelRigidBody = gameObject.AddComponent<Rigidbody>();
            _levelRigidBody.useGravity = false;
            _levelRigidBody.mass = 200;
            _levelRigidBody.isKinematic = true;
            _levelRigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            _levelRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            if (!_isCanFly) {
                return;
            }
            _curentPosition = _splineController.BezierSpline.MoveAlongSpline(ref _distanceTraveled, SPEED * Time.fixedDeltaTime, 50) * -1;
            _levelRigidBody.MovePosition(_curentPosition);
            float overclocking = Vector3.Distance(_curentPosition, _startPosition) / 75;
            float multiplyCoefficient = overclocking > 1f ? 1.0f : overclocking;
            Time.timeScale = 1.0f + 1.5f * multiplyCoefficient;
        }
    }
}