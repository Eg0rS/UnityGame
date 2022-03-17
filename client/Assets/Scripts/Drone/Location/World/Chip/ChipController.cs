using Adept.Logger;
using Drone.Location.Interface;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.ChipModel;
using UnityEngine;

namespace Drone.Location.World.Chip
{
    public class ChipController : MonoBehaviour, IWorldObjectController<ChipModel>, IInteractiveObject
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ChipController>();
        public WorldObjectType ObjectType { get; }

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
            gameObject.SetActive(false);
        }
    }
}