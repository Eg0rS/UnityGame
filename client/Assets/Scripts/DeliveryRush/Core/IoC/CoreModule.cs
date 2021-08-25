using DeliveryRush.Descriptor.Service;
using IoC.Api;
using JetBrains.Annotations;

namespace DeliveryRush.Core.IoC
{
    public class CoreModule : IIoCModule
    {
        public void Configure([NotNull] IIoCContainer container)
        {
            container.RegisterSingleton<DescriptorLoader>();
        }
    }
}