using Adept.Logger;
using AgkUI.Binding.Attributes;
using UnityEngine;

namespace DronDonDon.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI/Panel/pfMainScreenPanel@embeded";
        
        [UICreated]
        public void Init()
        {
            _logger.Debug("MainMenuPanel start init");
        }
    }
}