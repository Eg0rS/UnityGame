using DronDonDon.Location.Model;
using AgkCommons.Event;
using DronDonDon.Location.Model.BaseModel;
using DronDonDon.Location.Model.Obstacle;
using IoC.Attribute;
using UnityEngine;
using IoC.Extension;

namespace DronDonDon.Location.World.Obstacle
{
    public class ObstacleController : MonoBehaviour,  IWorldObjectController<ObstacleModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(ObstacleModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}