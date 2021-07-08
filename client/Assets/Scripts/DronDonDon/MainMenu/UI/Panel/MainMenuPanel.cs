using Adept.Logger;
using AgkUI.Binding.Attributes;
using DronDonDon.Core;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI/Panel/pfMainScreenPanel@embeded";
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        [UICreated]
        public void Init()
        {
            _overlayManager.Require().HideLoadingOverlay(true);
            _logger.Debug("MainMenuPanel start init");
        }
    }
}