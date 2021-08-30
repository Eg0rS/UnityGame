using DeliveryRush.Location.Service;
using DeliveryRush.Location.World.Dron.IoC;
using DeliveryRush.Location.World.Dron.Service;
using IoC.Api;

namespace DeliveryRush.Location.World.Dron.Module
{
    public class DronModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<GestureService>();
            container.RegisterSingleton<DronService>();
            container.RegisterSingleton<DronRepository>();
            container.RegisterSingleton<DronDescriptorRegistry>();
        }
    }
}