using IoC.Api;
using DronDonDon.Billing.Service;
using DronDonDon.Billing.Event;

namespace DronDonDon.Billing.IoC
{
    public class BillingModule: IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<BillingService>();
            container.RegisterSingleton<BillingRepository>();
            container.RegisterSingleton<BillingEvent>();
        } 
    }
}