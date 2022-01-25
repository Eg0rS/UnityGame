using Drone.Location.Model;
using Drone.Location.Model.StartPlatform;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.StartPlatform
{
    public class StartPlatformController : MonoBehaviour, IWorldObjectController<StartPlatformModel>
    {
        public WorldObjectType ObjectType { get; }

        [Inject]
        private DroneWorld _droneWorld;

        public void Init(StartPlatformModel model)
        {
            _droneWorld.Player.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        }
    }
}