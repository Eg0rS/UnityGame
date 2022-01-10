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
            container.RegisterSingleton<LocationService>();
            container.RegisterSingleton<LocationBuilderManager>();
            container.RegisterSingleton<CreateObjectService>();
            container.RegisterSingleton<GameOverlayManager>();
            
            container.RegisterSingleton<ControlService>(null, ScopeType.SCREEN);
            
            
            container.RegisterSingleton<ObstacleService>(null, ScopeType.SCREEN);
            
            //должен быть создан самым последним 
            container.RegisterSingleton<GameService>(null, ScopeType.SCREEN);
        }
    }
}