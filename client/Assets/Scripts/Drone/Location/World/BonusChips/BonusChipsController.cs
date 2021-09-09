using Drone.Location.Model;
using Drone.Location.Model.BonusChips;
using UnityEngine;

namespace Drone.Location.World.BonusChips
{
    public class BonusChipsController : MonoBehaviour, IWorldObjectController<BonusChipsModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(BonusChipsModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}