using DronDonDon.Core.L10n;
using DronDonDon.Descriptor.Service;
using IoC.Api;
using JetBrains.Annotations;

namespace DronDonDon.Core.IoC
{
    public class CoreModule : IIoCModule
    {
        public void Configure([NotNull] IIoCContainer container)
        {
            container.RegisterSingleton<L10nLoader>();
            container.RegisterSingleton<DescriptorLoader>();
        }
    }
}