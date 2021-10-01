using Drone.Location.Model.BaseModel;
using Drone.Location.Service.Builder;
using Drone.Location.World.SpeedBooster.Descriptor;
using IoC.Attribute;

namespace Drone.Location.Model.SpeedBooster
{
    public class SpeedBoosterModel : PrefabModel
    {
        [Inject]
        private BoosterService _boosterService;
        public float SpeedBoost { get; private set; }
        public float AccelerationBoost { get; private set; }
        public float NeedsEnergy { get; private set; }
        public float Duration { get; private set; }
        
        public void Awake()
        {
            ObjectType = WorldObjectType.SPEED_BUSTER;

            BoosterDescriptor descriptor = _boosterService.GetDescriptorById("SpeedBooster");
            SpeedBoost = descriptor._params["SpeedBoost"];
            SpeedBoost = descriptor._params["AccelerationBoost"];
            SpeedBoost = descriptor._params["NeedsEnergy"];
            SpeedBoost = descriptor._params["Duration"];
        }
    }
}