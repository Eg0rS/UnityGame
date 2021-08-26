using AgkCommons.CodeStyle;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Screens.Service;
using DeliveryRush.Location.UI;
using IoC.Attribute;
using RSG;

namespace DeliveryRush.Location.Service
{
    [Injectable]
    public class GameOverlayManager
    {
        [Inject]
        private UIService _uiService;

        [Inject]
        private ScreenStructureManager _screenStructureManager;

        private GameOverlay _gameOverlay;

        public IPromise LoadGameOverlay(DronStats dronStats)
        {
            return _uiService.Create<GameOverlay>(UiModel.Create<GameOverlay>(dronStats)).Then(Attach);
        }

        public bool GameOverlayEnabled
        {
            get { return _gameOverlay.gameObject.activeSelf; }
            set
            {
                if (_gameOverlay == null) {
                    return;
                }
                _gameOverlay.gameObject.SetActive(value);
            }
        }
        
        private IPromise Attach(GameOverlay arg)
        {
            Promise promise = new Promise();
            _gameOverlay = arg;
            _screenStructureManager.AttachToSafeArea(_gameOverlay.gameObject);
            GameOverlayEnabled = true;
            promise.Resolve();
            return promise;
        }
    }
}