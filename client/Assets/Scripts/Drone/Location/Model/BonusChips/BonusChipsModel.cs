using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.BonusChips
{
    public class BonusChipsModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.BONUS_CHIPS;
        }
    }
}