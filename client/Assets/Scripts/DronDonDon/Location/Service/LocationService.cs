using AgkCommons.CodeStyle;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService 
    {
        private const string LOCATION_PREFAB = "World/Location/pfLocation@embeded";
        [Inject]
        private ScreenManager _screenManager;   
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager; 
        public void StartGame()
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld();
        }
        private void CreatedWorld()
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(LOCATION_PREFAB)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }
    }
}