using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.Finish;
using UnityEngine;

namespace DeliveryRush.Location.World.Finish
{
    public class FinishController : MonoBehaviour, IWorldObjectController<FinishModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(FinishModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}