using DronDonDon.Game.Levels.Service;
using IoC.Api;

namespace DronDonDon.Game.Levels.IoC
{
    public class LevelsModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<LevelService>();
            container.RegisterSingleton<ProgressRepository>();
        }
    }
}