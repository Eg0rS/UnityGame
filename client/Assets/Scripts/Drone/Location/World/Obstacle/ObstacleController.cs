using Drone.Location.Model;
using Drone.Location.Model.Obstacle;
using UnityEngine;

namespace Drone.Location.World.Obstacle
{
    public class ObstacleController : MonoBehaviour, IWorldObjectController<ObstacleModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(ObstacleModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}