using AgkCommons.Extension;
using Drone.Location.Event;
using Drone.Location.Service;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;

namespace Drone.Location.World.Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private const string EXPLOSION_PATH = "Particles/ptExplosion@embeded";
        private const string SPARKS_PATH = "Particles/ptSparks@embeded";
        [Inject]
        private DroneWorld _gameWorld;
        [Inject]
        private LoadLocationObjectService _objectService;
        private GameObject _explosionParticle;
        private GameObject _sparksParticle;
        private GameObject _prefabMesh;

        private GameObject _particles;

        public void Configure()
        {
            this.Inject();
            _objectService.LoadResource<GameObject>(EXPLOSION_PATH).Then(go => _explosionParticle = go);
            _objectService.LoadResource<GameObject>(SPARKS_PATH).Then(go => _sparksParticle = go);
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT_BEGIN, OnObstacleContactBegin);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
        }

        private void OnStartGame(InGameEvent obj)
        {
            _particles.SetActive(true);
        }

        private void OnObstacleContactBegin(ObstacleEvent obj)
        {
            Instantiate(_sparksParticle, transform);
        }

        private void OnEndGame(InGameEvent obj)
        {
            if (obj.EndGameReason != EndGameReasons.CRUSH) {
                return;
            }
            _particles.SetActive(false);
            Instantiate(_explosionParticle, transform);
            _prefabMesh.SetActive(false);
        }

        public void SetMesh()
        {
            _prefabMesh = gameObject.GetChildren()[0];
        }

        public GameObject Particles
        {
            set { _particles = value; }
        }
    }
}