using DronDonDon.Location.Model;
using DronDonDon.Location.Model.ShieldBooster;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.ShieldBooster
{
    public class ShieldBoosterController : MonoBehaviour,  IWorldObjectController<ShieldBoosterModel >
    {
        public WorldObjectType ObjectType { get; private set; }
        public void Init(ShieldBoosterModel  model)
        {
            ObjectType = model.ObjectType;
        }
    }
}