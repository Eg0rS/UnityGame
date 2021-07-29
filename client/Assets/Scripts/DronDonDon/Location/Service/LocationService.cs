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
        [Inject]
        private ScreenManager _screenManager;  
        
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        private string _nameSelectedLevel;

        public string NameSelectedLevel
        {
            set => _nameSelectedLevel = value;
        }

        public void StartGame()
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(_nameSelectedLevel);
        }
        private void CreatedWorld(string nameSelectedLevel)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(nameSelectedLevel)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }
    }
}