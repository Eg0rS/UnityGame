using DeliveryRush.Billing.Descriptor;
using DeliveryRush.Billing.Event;
using DeliveryRush.Billing.Model;
using DeliveryRush.Billing.Service;
using IoC.Api;

namespace DeliveryRush.Billing.IoC
{
    public class BillingModule: IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<BillingService>();
            container.RegisterSingleton<BillingRepository>();
            container.RegisterSingleton<BillingEvent>();
            container.RegisterSingleton<PlayerResourceModel>();
            container.RegisterSingleton<BillingDescriptorRegistry>();
            container.RegisterSingleton<BillingDescriptor>();
        } 
    }
}