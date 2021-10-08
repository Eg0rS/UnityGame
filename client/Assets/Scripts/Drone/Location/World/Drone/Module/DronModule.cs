using Drone.Location.World.Drone.IoC;
using Drone.Location.World.Drone.Service;
using IoC.Api;

namespace Drone.Location.World.Drone.Module
{
    public class DronModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<DronControlService>();
            container.RegisterSingleton<DroneService>();
            container.RegisterSingleton<DronDescriptorRegistry>();
        }
    }
}