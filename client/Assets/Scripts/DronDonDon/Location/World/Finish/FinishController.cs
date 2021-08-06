using DronDonDon.Location.Model;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.Finish;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.Finish
{
    public class FinishController : MonoBehaviour,  IWorldObjectController<FinishModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        public void Init(FinishModel  model)
        {
            ObjectType = model.ObjectType;
        }
    }
}