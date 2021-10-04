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
            SpeedBoost = (float) descriptor._params["SpeedBoost"];
            AccelerationBoost = (float) descriptor._params["AccelerationBoost"];
            NeedsEnergy = (float) descriptor._params["NeedsEnergy"];
            Duration = (float) descriptor._params["Duration"];
        }
    }
}