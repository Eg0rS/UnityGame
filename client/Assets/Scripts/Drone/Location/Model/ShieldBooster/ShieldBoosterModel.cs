using Drone.Location.Model.BaseModel;
using Drone.Location.Service.Builder;
using IoC.Attribute;

namespace Drone.Location.Model.ShieldBooster
{
    public class ShieldBoosterModel : PrefabModel
    {
        [Inject]
        private BoosterService _boosterService;
        public float Duration { get; private set; }
        public void Awake()
        {
            ObjectType = WorldObjectType.SHIELD_BUSTER;
           
        }
    }
}