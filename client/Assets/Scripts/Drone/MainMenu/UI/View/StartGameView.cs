using Adept.Logger;
using AgkUI.Binding.Attributes;
using Drone.Core.UI.Builder;
using Drone.Core.UI.View;
using Drone.MainMenu.UI.Panel;

// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local

namespace Drone.MainMenu.UI.View
{
    [UIController(PREFAB)]
    public class StartGameView : ExpandedView
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<StartGameView>();

        private const string PREFAB = "StartGameView";

        private MainMenuPanel _mainMenuPanel;

        [UICreated]
        public void Init()
        {
        }

        
        public void Load(UIScreenPanelLoader uiScreenPanelLoader)
        {
            uiScreenPanelLoader.Add<MainMenuPanel>((mainMenu) => {
                _mainMenuPanel = mainMenu;
                _mainMenuPanel.Init();
            });
        }
    }
}