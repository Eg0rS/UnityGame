using DeliveryRush.LevelMap.Levels.Repository;
using DeliveryRush.LevelMap.Levels.Service;
using IoC.Api;

namespace DeliveryRush.LevelMap.Levels.IoC
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