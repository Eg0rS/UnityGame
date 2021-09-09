using Drone.Settings.Service;
using IoC.Api;

namespace Drone.Settings.IoC
{
    public class SettingsModule : IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<SettingsService>();
            container.RegisterSingleton<SettingsRepository>();
        }
    }
}