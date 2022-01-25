using Drone.Descriptor.Loader;
using Drone.Descriptor.Loader.Interfaces;
using Drone.Descriptor.Service;
using IoC.Api;

namespace Drone.Descriptor.IoC
{
    public class DescriptorModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            DescriptorRegistry registry = DescriptorRegistry.Instance;

            container.RegisterSingleton<IDescriptorLoader, LocalDescriptorLoader>();

            container.RegisterSingleton<DescriptorRegistry>(() => DescriptorRegistry.Instance);
            container.RegisterSingleton<LevelsDescriptors>(() => registry.GetSingleDescriptor<LevelsDescriptors>());
            container.RegisterSingleton<TileDescriptors>(() => registry.GetSingleDescriptor<TileDescriptors>());
            container.RegisterSingleton<ObstacleDescriptors>(() => registry.GetSingleDescriptor<ObstacleDescriptors>());
        }

        private void RegisterCollection<T>(IIoCContainer container, string collectionName)
        {
            DescriptorRegistry registry = DescriptorRegistry.Instance;
            container.RegisterSingleton<DescriptorCollection<T>>(() => registry.RequireCollection<T>(collectionName));
        }
    }
}