using DeliveryRush.Location.Model.BaseModel;

namespace DeliveryRush.Location.Model.Obstacle
{
    public class ObstacleModel : PrefabModel
    {
        public float Damage { get; private set; }
        public void Awake()
        {
            ObjectType = WorldObjectType.OBSTACLE;
            Damage = 13f;
        }
    }
}