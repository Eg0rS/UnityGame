using Drone.Location.Service.Builder;
using Drone.Location.World.SpeedBooster.IoC;
using IoC.Api;

namespace Drone.Location.IoC
{
    public class BoosterModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<BoosterDescriptorRegistry>();
            container.RegisterSingleton<BoosterService>();
        }
    }
}