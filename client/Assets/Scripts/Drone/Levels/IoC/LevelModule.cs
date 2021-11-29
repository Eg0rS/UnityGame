using Drone.Levels.Repository;
using Drone.Levels.Service;
using IoC.Api;

namespace Drone.Levels.IoC
{
    public class LevelModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<ProgressRepository>();
            container.RegisterSingleton<LevelService>();
        }
    }
}