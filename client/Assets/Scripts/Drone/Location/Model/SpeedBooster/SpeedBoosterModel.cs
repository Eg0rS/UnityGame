using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.SpeedBooster
{
    public class SpeedBoosterModel : PrefabModel
    {
        public float SpeedBoost { get; private set; }
        public float AccelerationBoost { get; private set; }
        public float NeedsEnergy{ get; private set; }
        public float Duration{ get; private set; }

        public void Awake()
        {
            ObjectType = WorldObjectType.SPEED_BUSTER;
            SpeedBoost = 1.5f;
            AccelerationBoost = 1.5f;
            NeedsEnergy = 3;
            Duration = 4;
        }
    }
}