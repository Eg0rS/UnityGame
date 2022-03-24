using Adept.Logger;
using Drone.Location.Interface;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.ChipModel;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Chip
{
    public class ChipController : MonoBehaviour, IWorldObjectController<ChipModel>, IInteractiveObject
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ChipController>();
        public WorldObjectType ObjectType { get; }

        [Inject]
        private DroneWorld _droneWorld;
        
        public void Init(ChipModel model)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            WorldObjectType objectType = other.gameObject.GetComponentInParent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.PLAYER) {
                _logger.Warn("Enter non-player Collider.");
                Debug.LogWarning(gameObject.name);
                return;
            }
            _droneWorld.Dispatch(new InGameEvent(InGameEvent.CHIP_UP));
            gameObject.SetActive(false);
        }
    }
}