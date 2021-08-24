using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.BonusChips;
using UnityEngine;

namespace DeliveryRush.Location.World.BonusChips
{
    public class BonusChipsController : MonoBehaviour,  IWorldObjectController<BonusChipsModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        public void Init(BonusChipsModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}