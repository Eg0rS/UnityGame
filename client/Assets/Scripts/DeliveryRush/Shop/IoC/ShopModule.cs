using DeliveryRush.Shop.Descriptor;
using DeliveryRush.Shop.Service;
using IoC.Api;

namespace DeliveryRush.Shop.IoC
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