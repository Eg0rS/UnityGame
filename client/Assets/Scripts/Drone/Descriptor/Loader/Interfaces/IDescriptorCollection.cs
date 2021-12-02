using System.Collections.Generic;

namespace Drone.Descriptor.Loader.Interfaces
{
    public interface IDescriptorCollection
    {
        void PutAll(Dictionary<string, object> map);
        void Clear();
    }
}