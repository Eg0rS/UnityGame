using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using UnityEngine;

namespace DronDonDon.Shop.UI
{
    [UIController("UI/Dialog/ScrollView@embeded")]
    public class ScrollController : MonoBehaviour
    {

        public ListPositionCtrl Control;
        [UICreated]
        private void Init(List<ShopItemPanel> _ShopItemPanels)
        {
            ListPositionCtrl control = gameObject.GetComponent<ListPositionCtrl>();
            Control = control;
            foreach (var itemPanel in _ShopItemPanels)
            {
                control.listBoxes.Add(itemPanel.GetComponent<ListBox>());
            }
        }
    }
}