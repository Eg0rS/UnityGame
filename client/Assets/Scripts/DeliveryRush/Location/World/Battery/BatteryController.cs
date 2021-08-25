using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.Battery;
using UnityEngine;

namespace DeliveryRush.Location.World.Battery
{
    public class BatteryController : MonoBehaviour, IWorldObjectController<BatteryModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(BatteryModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}