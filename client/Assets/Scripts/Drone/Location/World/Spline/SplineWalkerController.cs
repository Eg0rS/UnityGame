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

        private float _distanceTraveled = 0f;
        private bool _isCanFly = false;

        public void Init(SplineWalkerModel model)
        {
            ConfigurateRigidBody();
            _splineController = _gameWorld.RequireObjectComponent<SplineController>();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
        }

        private void OnEndGame(InGameEvent obj)
        {
            _isCanFly = false;
        }

        private void OnStartGame(InGameEvent obj)
        {
            _isCanFly = true;
        }

        private void ConfigurateRigidBody()
        {
            _levelRigidBody = gameObject.AddComponent<Rigidbody>();
            _levelRigidBody.useGravity = false;
            _levelRigidBody.mass = 200;
            _levelRigidBody.isKinematic = true;
            _levelRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            if (!_isCanFly) {
                return;
            }
            Vector3 position = _splineController.BezierSpline.MoveAlongSpline(ref _distanceTraveled, SPEED * Time.fixedDeltaTime, 50);
            position *= -1;
            _levelRigidBody.MovePosition(position);
            BezierSpline.Segment segment = _splineController.BezierSpline.GetSegmentAt(_distanceTraveled);
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.CHANGE_SPLINE_SEGMENT, segment));
        }
    }
}