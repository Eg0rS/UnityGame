using Adept.Logger;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.WorldGeomertyRotation;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.WorldGeomertyRotation
{
    public class WorldGeometryRotationController : MonoBehaviour, IWorldObjectController<WorldGeometryRotationModel>
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<WorldGeometryRotationController>();
        [Inject]
        private DroneWorld _gameWorld;
        public void Init(WorldGeometryRotationModel model)
        {
        }

        public WorldObjectType ObjectType { get; }

        private void OnTriggerEnter(Collider other)
        {
            WorldObjectType objectType = other.gameObject.GetComponentInParent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.PLAYER) {
                _logger.Warn("Enter non-player Collider.");
                Debug.LogWarning(gameObject.name);
                return;
            }
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.CHANGE_TILE));
        }
    }
}