using System;
using System.Collections;
using AgkCommons.Event;
using Drone.Location.Model;
using Drone.Location.Model.Spline;
using Drone.Location.Service.Control;
using UnityEngine;
using DG.Tweening;
using Drone.Location.Service.Control.Spline;
using Drone.Location.Service.Game.Event;
using Drone.World;
using IoC.Attribute;

namespace Drone.Location.World.Spline
{
    public class SplineWalkerController : MonoBehaviour, IWorldObjectController<SplineWalkerModel>
    {
        public WorldObjectType ObjectType { get; }
        private Rigidbody _levelRigidBody;
        private float m_normalizedT = 0f;
        private SplineController _splineController;
        [Inject]
        private GameWorld _gameWorld;
        private Coroutine _walking;
        float targetSpeed = 3;
        private bool start = false;

        public void Init(SplineWalkerModel model)
        {
            ConfigurateRigidBody();
            _splineController = _gameWorld.RequireObjectComponent<SplineController>();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
        }

        private void OnStartGame(InGameEvent obj)
        {
            start = true;
            Physics.autoSimulation = true;
        }

        private void ConfigurateRigidBody()
        {
            Physics.autoSimulation = false;
            _levelRigidBody = gameObject.AddComponent<Rigidbody>();
            _levelRigidBody.useGravity = false;
        }
        
        private void FixedUpdate()
        {
            if (!start) {
                return;
            }
            Vector3 targetPos = _splineController.BezierSpline.MoveAlongSpline(ref m_normalizedT, targetSpeed);
            _levelRigidBody.MovePosition(targetPos);
        }

        private IEnumerator Walk()
        {
            while (true) {
                Vector3 newPos = _splineController.BezierSpline.MoveAlongSpline(ref m_normalizedT, 3);
                //Vector3 newPos = new Vector3(0, 0, _levelRigidBody.position.z + 10);
                _levelRigidBody.DOMove(newPos * -1, 1f);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}