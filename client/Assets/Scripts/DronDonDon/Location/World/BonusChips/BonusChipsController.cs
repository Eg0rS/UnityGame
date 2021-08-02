using DronDonDon.Location.Model;
using DronDonDon.Location.Model.BonusChips;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.BonusChips
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