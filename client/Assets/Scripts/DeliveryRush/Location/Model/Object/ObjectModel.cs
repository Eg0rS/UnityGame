using DeliveryRush.Location.Model.BaseModel;

namespace DeliveryRush.Location.Model.Object
{
    public class ObjectModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.OBSTACLE;
        }
    }
}