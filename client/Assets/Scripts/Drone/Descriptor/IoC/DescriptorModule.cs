using Drone.Descriptor.Service;
using IoC.Api;

namespace Drone.Descriptor.IoC
{
    public class DescriptorModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            DescriptorRegistry registry = DescriptorRegistry.Instance;
            container.RegisterSingleton<DescriptorRegistry>(() => DescriptorRegistry.Instance);
            
        }
    }
}