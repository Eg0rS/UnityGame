using Drone.PowerUp.Service;
using IoC.Api;

namespace Drone.PowerUp.IoC
{
    public class PowerUpModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<PowerUpService>();
        }
    }
}