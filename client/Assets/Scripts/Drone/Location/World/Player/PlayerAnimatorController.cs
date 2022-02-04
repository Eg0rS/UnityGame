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

        public void Configure()
        {
            
            this.Inject();
            _objectService.LoadResource<GameObject>(EXPLOSION_PATH).Then(go => _explosionParticle = go);
            _objectService.LoadResource<GameObject>(SPARKS_PATH).Then(go => _sparksParticle = go);
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT_BEGIN, OnObstacleContactBegin);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);
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
            Instantiate(_explosionParticle, transform);
            _prefabMesh.SetActive(false);
        }

        public void SetMesh()
        {
            _prefabMesh = gameObject.GetChildren()[0];
        }
    }
}