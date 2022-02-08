using Adept.Logger;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Finish;
using Drone.Location.Service.Game.Event;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Finish
{
    public class FinishController : MonoBehaviour, IWorldObjectController<FinishModel>
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<FinishController>();
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private DroneWorld _gameWorld;

        public void Init(FinishModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnTriggerEnter(Collider other)
        {
            WorldObjectType objectType = other.gameObject.GetComponentInParent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.PLAYER) {
                _logger.Warn("Enter non-player Collider.");
                Debug.LogWarning(gameObject.name);
                return;
            }
            _gameWorld.Dispatch(new InGameEvent(InGameEvent.END_GAME, EndGameReasons.VICTORY));
        }
    }
}