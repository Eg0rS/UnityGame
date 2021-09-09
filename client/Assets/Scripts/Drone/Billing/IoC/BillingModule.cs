using Drone.Billing.Descriptor;
using Drone.Billing.Event;
using Drone.Billing.Model;
using Drone.Billing.Service;
using IoC.Api;

namespace Drone.Billing.IoC
{
    public class BillingModule : IIoCModule
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