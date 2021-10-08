using Drone.Location.Service;
using Drone.Location.World.Dron.IoC;
using Drone.Location.World.Dron.Service;
using IoC.Api;

namespace Drone.Location.World.Dron.Module
{
    public class DronModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<DronService>();
            container.RegisterSingleton<DronDescriptorRegistry>();
        }
    }
}