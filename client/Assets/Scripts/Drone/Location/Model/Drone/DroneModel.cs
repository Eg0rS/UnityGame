using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Drone
{
    public class DroneModel : PrefabModel
    {
        public float Mobility { get; private set; }
        public float Durability { get; private set; }
        public float Energy { get; private set; }

        public DroneModel()
        {
            ObjectType = WorldObjectType.DRON;
        }

        public void SetDroneParameters(float mobility, float durability, float energy)
        {
            Mobility = mobility;
            Durability = durability;
            Energy = energy;
        }
    }
}