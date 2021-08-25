using DeliveryRush.Resource.Repository;
using DeliveryRush.Resource.Service;
using IoC.Api;

namespace DeliveryRush.Resource.IoC
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