using DronDonDon.Game.Levels.Repository;
using DronDonDon.Game.Levels.Service;
using IoC.Api;

namespace DronDonDon.Game.Levels.Module
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