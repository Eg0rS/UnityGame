using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.ShieldBooster;
using UnityEngine;

namespace DeliveryRush.Location.World.ShieldBooster
{
    public class ShieldBoosterController : MonoBehaviour, IWorldObjectController<ShieldBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(ShieldBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}