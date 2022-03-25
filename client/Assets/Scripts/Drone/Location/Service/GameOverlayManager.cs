using AgkCommons.CodeStyle;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Screens.Service;
using Drone.Location.UI;
using IoC.Attribute;
using RSG;

namespace Drone.Location.Service
{
    [Injectable]
    public class GameOverlayManager
    {
        [Inject]
        private UIService _uiService;

        [Inject]
        private ScreenStructureManager _screenStructureManager;

        private GameHUD _gameHUD;

        public void LoadGameOverlay()
        {
            _uiService.Create<GameHUD>(UiModel.Create<GameHUD>()).Then(Attach);
        }

        private void Attach(GameHUD arg)
        {
            _gameHUD = arg;
            _screenStructureManager.AttachToSafeArea(_gameHUD.gameObject);
        }
    }
}