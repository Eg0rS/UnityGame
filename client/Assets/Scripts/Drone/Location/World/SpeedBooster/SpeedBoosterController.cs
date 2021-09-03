using Drone.Location.Model;
using Drone.Location.Model.SpeedBooster;
using UnityEngine;

namespace Drone.Location.World.SpeedBooster
{
    public class SpeedBoosterController : MonoBehaviour, IWorldObjectController<SpeedBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(SpeedBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}