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
            if (objectType == WorldObjectType.DRON) {
                ContactPoint[] contactPoints = otherCollision.contacts;
                if (_onActiveShield) {
                    return;
                }
                _cameraNoise.m_AmplitudeGain += _crashNoise;
                foreach (ContactPoint contact in contactPoints) {
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASH, DroneParticles.ptSparks, contact, otherCollision.transform));
                }
                Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
            }
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= _crashNoise;
        }
    }
}