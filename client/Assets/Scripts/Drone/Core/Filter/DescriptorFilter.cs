using Adept.Logger;
using Drone.Descriptor;
using Drone.Descriptor.Loader.Interfaces;
using IoC.Attribute;

namespace Drone.Core.Filter
{
    public class DescriptorFilter : IAppFilter
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DescriptorFilter>();
        [Inject]
        private IDescriptorLoader _localDescriptorLoader;

        public void Run(AppFilterChain chain)
        {
            _localDescriptorLoader.AddDescriptor<LevelsDescriptors>(Descriptors.LEVELS).Load().Done((() => chain.Next()));
        }
    }
}