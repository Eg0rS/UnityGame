using System.Collections.Generic;
using DeliveryRush.Billing.Descriptor;

namespace DeliveryRush.Billing.IoC
{
    public class BillingDescriptorRegistry
    {
        private readonly List<BillingDescriptor> _billingDescriptors;

        public List<BillingDescriptor> BillingDescriptors
        {
            get => _billingDescriptors;
        }

        public BillingDescriptorRegistry()
        {
            _billingDescriptors = new List<BillingDescriptor>();
        }
    }
}