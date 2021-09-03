using System.Collections.Generic;
using Drone.Billing.Descriptor;

namespace Drone.Billing.IoC
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