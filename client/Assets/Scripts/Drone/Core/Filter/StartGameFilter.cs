using AgkUI.Screens.Service;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;

namespace Drone.Core.Filter
{
    public class StartGameFilter : IAppFilter
    {
        [Inject]
        private ScreenManager _screenManager;
        public void Run(AppFilterChain chain)
        {
            _screenManager.LoadScreen<MainMenuScreen>();
            chain.Next();
        }
    }
}