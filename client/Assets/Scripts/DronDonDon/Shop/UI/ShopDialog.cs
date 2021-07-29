using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using DronDonDon.Core.UI.Dialog;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkUI.Element.Text;
using UnityEngine;
using UnityEngine.UI;


namespace DronDonDon.Shop.UI
{
    [UIController("UI/Dialog/pfShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class ShopDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ShopDialog>();
        [UIComponentBinding("ScrollView")] 
        private ListPositionCtrl _scrollControl;
        
        [UICreated]
        public void Init()
        {
           
        }

        [UIOnClick("LeftContainer")]
        private void OnLeftClick()
        {
            _logger.Debug("clickleft");
            _scrollControl.SetUnitMove(1);
        }
        [UIOnClick("RightContainer")]
        private void OnRightClick()
        {
            _logger.Debug("clickrekt");
            _scrollControl.SetUnitMove(-1);
        }
    }
}