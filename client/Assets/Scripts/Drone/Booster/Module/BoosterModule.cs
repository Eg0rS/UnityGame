using Drone.Booster.Service;
using IoC.Api;

namespace Drone.Booster.Module
{
    public class BoosterModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<BoosterService>();
        }
    }
}