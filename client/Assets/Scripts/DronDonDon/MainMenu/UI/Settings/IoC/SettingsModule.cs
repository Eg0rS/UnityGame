using DronDonDon.MainMenu.UI.Settings.Service;
using IoC.Api;

namespace DronDonDon.MainMenu.UI.Settings.IoC
{
    public class SettingsModule: IIoCModule
    {
        public void Configure(IIoCContainer container)
        {
            container.RegisterSingleton<SettingsService>();
            container.RegisterSingleton<SettingsRepository>();
        }
    }
}