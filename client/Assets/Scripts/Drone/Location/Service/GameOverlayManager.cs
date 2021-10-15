using AgkCommons.CodeStyle;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Screens.Service;
using Drone.Location.UI;
using Drone.Location.World.Drone.Model;
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

        private GameOverlay _gameOverlay;

        public IPromise LoadGameOverlay()
        {
            return _uiService.Create<GameOverlay>(UiModel.Create<GameOverlay>()).Then(Attach);
        }
        
        private IPromise Attach(GameOverlay arg)
        {
            Promise promise = new Promise();
            _gameOverlay = arg;
            _screenStructureManager.AttachToOverlay(_gameOverlay.gameObject);
            promise.Resolve();
            return promise;
        }
    }
}