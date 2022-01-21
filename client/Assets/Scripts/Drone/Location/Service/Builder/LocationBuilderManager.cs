using AgkCommons.CodeStyle;
using AgkUI.Screens.Service;
using IoC.Attribute;

namespace Drone.Location.Service.Builder
{
    [Injectable]
    public class LocationBuilderManager
    {

        [Inject]
        private LocationObjectCreateService _locationObjectCreateService;

        [Inject]
        private ScreenStructureManager _screenStructureManager;

        public LocationBuilder CreateDefault()
        {
            return LocationBuilder.Create(_locationObjectCreateService).Container(_screenStructureManager.ScreenWorldViewContainer.transform);
        }
    }
}