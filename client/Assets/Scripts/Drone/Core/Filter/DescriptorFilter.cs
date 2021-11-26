using Adept.Logger;
using Drone.Descriptor.Loader.Interfaces;
using Drone.Shop.Descriptor;
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
            _localDescriptorLoader.AddDescriptor<GameDescriptor>(Descriptors.LEVEL_SETTINGS)
                                 
                                  .Load()
                                  .Done();
        }
    }
}