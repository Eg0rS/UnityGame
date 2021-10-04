using Drone.Location.Service;
using Drone.Location.Service.Builder;
using Drone.Location.World.SpeedBooster.IoC;
using IoC.Api;

namespace Drone.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<GameService>(); 
            container.RegisterSingleton<LocationService>();
            container.RegisterSingleton<LocationBuilderManager>();
            container.RegisterSingleton<CreateObjectService>();
            container.RegisterSingleton<GameOverlayManager>();
        }
    }
}