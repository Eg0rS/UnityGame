using DeliveryRush.Location.Service;
using DeliveryRush.Location.Service.Builder;
using IoC.Api;

namespace DeliveryRush.Location.IoC
{
    public class LocationModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<LocationService>();
            container.RegisterSingleton<LocationBuilderManager>();
            container.RegisterSingleton<CreateObjectService>();
            container.RegisterSingleton<GameService>();
        }
    }
}