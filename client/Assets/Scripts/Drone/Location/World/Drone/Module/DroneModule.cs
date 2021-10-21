using Drone.Location.World.Drone.IoC;
using Drone.Location.World.Drone.Service;
using IoC.Api;
using IoC.Scope;

namespace Drone.Location.World.Drone.Module
{
    public class DroneModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<DroneService>();
            container.RegisterSingleton<DroneDescriptorRegistry>();
        }
    }
}