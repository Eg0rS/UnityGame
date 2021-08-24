using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.SpeedBooster;
using UnityEngine;

namespace DeliveryRush.Location.World.SpeedBooster
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