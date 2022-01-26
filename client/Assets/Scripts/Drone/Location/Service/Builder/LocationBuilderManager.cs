using AgkCommons.CodeStyle;
using AgkUI.Screens.Service;
using IoC.Attribute;

namespace Drone.Location.Service.Builder
{
    [Injectable]
    public class LocationBuilderManager
    {
        [Inject]
        private CreateLocationObjectService _createLocationObjectService;
        [Inject]
        private LoadLocationObjectService _loadLocationObjectService;

        [Inject]
        private ScreenStructureManager _screenStructureManager;

        public LocationBuilder CreateDefault()
        {
            return LocationBuilder.Create(_createLocationObjectService, _loadLocationObjectService)
                                  .Container(_screenStructureManager.ScreenWorldViewContainer.transform);
        }
    }
}