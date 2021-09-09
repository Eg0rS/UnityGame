using Drone.Core.Repository;
using Drone.Settings.Model;

namespace Drone.Settings.Service
{
    public class SettingsRepository : LocalPrefsSingleRepository<SettingsModel>
    {
        public SettingsRepository() : base("settingsRepository")
        {
        }
    }
}