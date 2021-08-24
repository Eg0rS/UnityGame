using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.Object;
using UnityEngine;

namespace DeliveryRush.Location.World.Object
{
    public class ObjectController : MonoBehaviour,  IWorldObjectController<ObjectModel >
    {
        public WorldObjectType ObjectType { get; private set; }
        public void Init(ObjectModel  model)
        {
            ObjectType = model.ObjectType;
        }
    }
}