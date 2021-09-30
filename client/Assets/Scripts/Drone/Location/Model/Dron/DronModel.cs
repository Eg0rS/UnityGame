using Drone.Location.Model.BaseModel;
using IoC.Util;

namespace Drone.Location.Model.Dron
{
    public class DronModel : PrefabModel, IoCProvider<DronModel>
    {
        
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