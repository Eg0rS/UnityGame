using Drone.Location.Model;
using Drone.Location.Model.Finish;
using UnityEngine;

namespace Drone.Location.World.Finish
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