using Drone.Location.Service;
using Drone.Location.Service.Builder;
using Drone.Location.Service.Control;
using Drone.Location.Service.Game;
using Drone.Location.Service.Obstacle;
using Drone.Obstacles.Service;
using IoC.Api;
using IoC.Scope;
using Tile.Service;

namespace Drone.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<LocationService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<LocationBuilderManager>(null, ScopeType.SCREEN);
            container.RegisterSingleton<LocationObjectCreateService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<LoadLocationObjectService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<GameOverlayManager>(null, ScopeType.SCREEN);

            container.RegisterSingleton<ControlService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<TileService>(null, ScopeType.SCREEN);
            
            container.RegisterSingleton<ObstacleService>(null, ScopeType.SCREEN);
            
            //
            container.RegisterSingleton<ObstaclesService>(null, ScopeType.SCREEN);
            //
            
            //должен быть создан самым последним 
            container.RegisterSingleton<GameService>(null, ScopeType.SCREEN);
        }
    }
}