using Drone.LevelMap.Levels.Repository;
using Drone.LevelMap.Levels.Service;
using Drone.LevelMap.Zones.IoC;
using IoC.Api;

namespace Drone.LevelMap.Levels.IoC
{
    public class LevelModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<ProgressRepository>();
            container.RegisterSingleton<LevelDescriptorRegistry>();
            container.RegisterSingleton<ZoneDescriptorRegistry>();
            container.RegisterSingleton<LevelService>();
            
        }
    }
}