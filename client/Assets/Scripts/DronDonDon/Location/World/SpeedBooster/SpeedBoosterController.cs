using DronDonDon.Location.Model;
using DronDonDon.Location.Model.SpeedBooster;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.SpeedBooster
{
    public class SpeedBoosterController : MonoBehaviour,  IWorldObjectController<SpeedBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        public void Init(SpeedBoosterModel  model)
        {
            ObjectType = model.ObjectType;
        }
    }
}