using RSG;

namespace Drone.Descriptor.Loader.Interfaces
{
    public interface IDescriptorLoader
    {
        IDescriptorLoader AddCollection<T>(string collectionName);
        IDescriptorLoader AddDescriptor<T>(string descriptorName);
        IPromise Load();
    }
}