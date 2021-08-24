using DeliveryRush.Core.Repository;
using DeliveryRush.Settings.Model;

namespace DeliveryRush.Settings.Service
{
    public class SettingsRepository : LocalPrefsSingleRepository<SettingsModel>
    {
        public SettingsRepository() : base("settingsRepository")    
        {
            
        }
    }
}