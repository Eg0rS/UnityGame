using Drone.LevelMap.Levels.Repository;
using Drone.LevelMap.Levels.Service;
using Drone.LevelMap.Regions.IoC;
using IoC.Api;

namespace Drone.LevelMap.Levels.IoC
{
    public class LevelsModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<ProgressRepository>();
            container.RegisterSingleton<LevelDescriptorRegistry>();
            container.RegisterSingleton<RegionDescriptorRegistry>();
            container.RegisterSingleton<LevelService>();
        }
    }
}