using AgkUI.Screens.Service;
using DronDonDon.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Core.Filter
{
    public class StartGameFilter : IAppFilter
    {
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        public void Run(AppFilterChain chain)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<MainMenuScreen>();
            chain.Next();
        }
    }
}