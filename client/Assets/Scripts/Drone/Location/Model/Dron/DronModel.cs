using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Dron
{
    public class DronModel : PrefabModel //IoCProvider<DronModel>
    {
        public float Mobility { get;}
        public float Durability { get;}
        public float Energy { get;}

        public DronModel(float mobility, float durability, float energy)
        {
            Mobility = mobility;
            Durability = durability;
            Energy = energy;
            ObjectType = WorldObjectType.DRON;
        }

        // public DronModel Get()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public DronModel Require()
        // {
        //     return this;
        // }
    }
}