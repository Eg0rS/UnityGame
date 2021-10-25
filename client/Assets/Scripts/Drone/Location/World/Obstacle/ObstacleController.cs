using Cinemachine;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Obstacle;
using Drone.Location.Service;
using Drone.Location.World.Drone;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Obstacle
{
    public class ObstacleController : MonoBehaviour, IWorldObjectController<ObstacleModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        private const float MAX_DISTANCE_DEPTH_COLLIDER = 0.034f;
        private const float TIME_FOR_DEAD = 1f;

        private float _crashNoise = 2;
        private float _crashNoiseDuration = 0.5f;

        private bool _onActiveShield;

        public void Init(ObstacleModel model)
        {
            ObjectType = model.ObjectType;
            CinemachineVirtualCamera droneCamera = _gameWorld.Require().GetDroneCamera();
            _cameraNoise = droneCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, EnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, DisableShield);
        }

        private void EnableShield(WorldEvent obj)
        {
            _onActiveShield = true;
        }

        private void DisableShield(WorldEvent obj)
        {
            _onActiveShield = false;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.DRON) {
                return;
            }
            ContactPoint[] contactPoints = otherCollision.contacts;
            if (_onActiveShield) {
                return;
            }
            _cameraNoise.m_AmplitudeGain += _crashNoise;
            foreach (ContactPoint contact in contactPoints) {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASH, DroneParticles.ptSparks, contact, otherCollision.transform));
            }
            if (IsDeadCrash(otherCollision)) {
                otherCollision.gameObject.SetActive(false);
                _gameWorld.Require()
                          .Dispatch(new WorldEvent(WorldEvent.CRASH, DroneParticles.ptExplosion, contactPoints[0], otherCollision.transform));
                Invoke(nameof(GameOver), TIME_FOR_DEAD);
            }
            Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
        }

        private bool IsDeadCrash(Collision otherCollision)
        {
            Physics.ComputePenetration(gameObject.GetComponent<Collider>(), transform.position, transform.rotation, otherCollision.collider,
                                       otherCollision.transform.position, otherCollision.transform.rotation, out Vector3 direction,
                                       out float distance);
            return distance >= MAX_DISTANCE_DEPTH_COLLIDER; //расстояние погружения одного коллайдера в другой
        }

        private void GameOver()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASHED, FailedReasons.Crashed));
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= _crashNoise;
        }
    }
}