using Drone.Shop.Descriptor;
using Drone.Shop.Service;
using IoC.Api;

namespace Drone.Shop.IoC
{
    public class ShopModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<ShopService>();
            container.RegisterSingleton<ShopDescriptor>();
        }
    }
}