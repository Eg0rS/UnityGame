using DeliveryRush.Game.Levels.Repository;
using DeliveryRush.Game.Levels.Service;
using IoC.Api;

namespace DeliveryRush.Game.Levels.IoC
{
    public class LevelsModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<ProgressRepository>();
            container.RegisterSingleton<LevelDescriptorRegistry>();
            container.RegisterSingleton<LevelService>();
        }
    }
}