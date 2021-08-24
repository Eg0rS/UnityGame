using DeliveryRush.Location.Model.BaseModel;

namespace DeliveryRush.Location.Model.Battery
{
    public class BatteryModel: PrefabModel
    {
        public float Energy { get; private set; }
        public void Awake()
        {
            ObjectType = WorldObjectType.Battery;
            Energy = 3f;
        }
    }
}