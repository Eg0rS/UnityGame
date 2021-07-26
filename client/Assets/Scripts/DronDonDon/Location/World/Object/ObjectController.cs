using DronDonDon.Location.Model;
using DronDonDon.Location.Model.Object;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.Object
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