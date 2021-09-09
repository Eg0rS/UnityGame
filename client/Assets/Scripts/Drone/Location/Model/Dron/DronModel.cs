using Drone.Location.Model.BaseModel;
using IoC.Util;

namespace Drone.Location.Model.Dron
{
    public class DronModel : PrefabModel, IoCProvider<DronModel>
    {
        public float SpeedShift = 2;
        public float durability = 10;
        public float Energy = 10;

        public void Awake()
        {
            ObjectType = WorldObjectType.DRON;
        }

        public DronModel Get()
        {
            throw new System.NotImplementedException();
        }

        public DronModel Require()
        {
            return this;
        }
    }
}