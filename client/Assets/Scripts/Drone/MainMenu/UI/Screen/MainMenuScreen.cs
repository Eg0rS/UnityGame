using Adept.Logger;
using AgkUI.Binding.Attributes;
using Drone.Core.UI.Builder;
using Drone.MainMenu.UI.View;
using UnityEngine;

namespace Drone.MainMenu.UI.Screen
{
    [UIController("UI/Screen/pfMainMenuScreen@embeded")]
    public class MainMenuScreen : MonoBehaviour
    {
        
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuScreen>();
        
        [UIComponentBinding("UI", true)]
        private StartGameView _startGameView;

        private void Start()
        {
            _logger.Debug("MainMenuScreen start load.");
            
            EnableAnimation();

            UIScreenPanelLoader uiItemLoader = CreatePanelLoader();
            uiItemLoader.Load(() => {
                _logger.Debug("MainMenuScreen finish load.");
            });
        }

        private UIScreenPanelLoader CreatePanelLoader()
        {
            UIScreenPanelLoader loader = gameObject.AddComponent<UIScreenPanelLoader>();
            _startGameView.Load(loader);
            return loader;
        }

        private void EnableAnimation()
        {
            Time.timeScale = 1;
        }
    }
}