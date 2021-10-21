using Drone.Location.Service;
using Drone.Location.Service.Builder;
using Drone.Location.World.Drone;
using IoC.Api;
using IoC.Scope;

namespace Drone.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<GameService>(); //todo брать в другой модуль
            container.RegisterSingleton<LocationService>();
            container.RegisterSingleton<LocationBuilderManager>();
            container.RegisterSingleton<CreateObjectService>();
            container.RegisterSingleton<GameOverlayManager>();

            container.RegisterSingleton<DroneAnimService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<BoosterService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<BatteryService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<ObstacleService>(null, ScopeType.SCREEN);
            container.RegisterSingleton<BonusChipService>(null, ScopeType.SCREEN);
        }
    }
}