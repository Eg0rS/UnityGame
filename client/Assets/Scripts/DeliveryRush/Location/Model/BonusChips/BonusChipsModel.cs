using DeliveryRush.Location.Model.BaseModel;

namespace DeliveryRush.Location.Model.BonusChips
{
    public class BonusChipsModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.BONUS_CHIPS;
        }
    }
}