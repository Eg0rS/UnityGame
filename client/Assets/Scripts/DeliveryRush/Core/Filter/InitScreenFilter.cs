using AgkUI.Screens.Service;
using DeliveryRush.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace DeliveryRush.Core.Filter
{
    public class InitScreenFilter : IAppFilter
    {
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        public void Run(AppFilterChain chain)
        {
            _screenManager.LoadScreen<InitScreen>();
            _overlayManager.Require().ShowPreloader();
            chain.Next();
        }
    }
}