using DeliveryRush.Location.Model.BaseModel;

namespace DeliveryRush.Location.Model.Finish
{
    public class FinishModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.FINISH;
        }
    }
}