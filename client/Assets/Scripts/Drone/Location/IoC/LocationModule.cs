using Drone.Location.Service;
using Drone.Location.Service.Builder;
using IoC.Api;
using IoC.Scope;

namespace Drone.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<LocationService>();
            container.RegisterSingleton<LocationBuilderManager>();
            container.RegisterSingleton<CreateObjectService>();
            container.RegisterSingleton<GameOverlayManager>();
            
            container.RegisterSingleton<GameService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<BoosterService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<ObstacleService>(null, ScopeType.SCREEN);
        }
    }
}