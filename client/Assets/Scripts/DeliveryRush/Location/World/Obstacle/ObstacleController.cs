using DeliveryRush.Location.Model;
using DeliveryRush.Location.Model.Obstacle;
using UnityEngine;

namespace DeliveryRush.Location.World.Obstacle
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