using DronDonDon.Core.Repository;
using DronDonDon.MainMenu.UI.Settings.Model;

namespace DronDonDon.MainMenu.UI.Settings.Service
{
    public class SettingsRepository : LocalPrefsSingleRepository<SettingsModel>
    {
        public SettingsRepository() : base("settingsRepository")    
        {
            
        }
    }
}