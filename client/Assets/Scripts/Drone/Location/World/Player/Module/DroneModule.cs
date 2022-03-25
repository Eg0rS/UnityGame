using Drone.Location.Service.Control.Drone.IoC;
using Drone.Location.World.Player.Service;
using IoC.Api;

namespace Drone.Location.Service.Control.Drone.Module
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