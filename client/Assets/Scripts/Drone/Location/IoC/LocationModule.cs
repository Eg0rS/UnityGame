using Drone.Location.Service;
using Drone.Location.Service.Builder;
using Drone.Location.Service.Control;
using Drone.Location.Service.Game;
using Drone.Location.Service.Obstacle;
using IoC.Api;
using IoC.Scope;

namespace Drone.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<LocationService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<LocationBuilderManager>(null, ScopeType.SCREEN);
            container.RegisterSingleton<CreateLocationObjectService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<LoadLocationObjectService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<GameOverlayManager>(null, ScopeType.SCREEN);

            container.RegisterSingleton<ControlService>(null, ScopeType.SCREEN);

            container.RegisterSingleton<ObstacleService>(null, ScopeType.SCREEN);

            //должен быть создан самым последним 
            container.RegisterSingleton<GameService>(null, ScopeType.SCREEN);
        }
    }
}