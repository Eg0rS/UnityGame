﻿using Drone.Descriptor.Service;
using IoC.Api;
using JetBrains.Annotations;

namespace Drone.Core.IoC
{
    public class CoreModule : IIoCModule
    {
        public void Configure([NotNull] IIoCContainer container)
        {
            container.RegisterSingleton<DescriptorLoader>();
        }
    }
}