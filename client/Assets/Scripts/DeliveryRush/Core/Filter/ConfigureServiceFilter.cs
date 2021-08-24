using DeliveryRush.Core.Service;
using IoC;

namespace DeliveryRush.Core.Filter
{
    public class ConfigureServiceFilter : IAppFilter
    {
        public void Run(AppFilterChain chain)
        {
            foreach (IConfigurable service in AppContext.ResolveCollection<IConfigurable>()) {
                service.Configure();
            }
            chain.Next();
        }
    }
}