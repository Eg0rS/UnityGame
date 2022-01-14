using AgkCommons.CodeStyle;
using AgkCommons.Resources;
using AgkUI.Screens.Service;
using IoC.Attribute;
using Tile.Service;

namespace Drone.Location.Service.Builder
{
    [Injectable]
    public class LocationBuilderManager
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private CreateObjectService _createService;

        [Inject]
        private TileService _tileService;
        
        [Inject]
        private ScreenStructureManager _screenStructureManager;

        public LocationBuilder CreateDefault()
        {
            return LocationBuilder.Create(_resourceService, _createService, _tileService).Container(_screenStructureManager.ScreenWorldViewContainer.transform);
        }
    }
}