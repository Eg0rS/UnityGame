using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using CircularScrollingList;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeliveryRush.Shop.UI
{
    [UIController("UI/Scrolls/pfScrollViewDroneStore@embeded")]
    public class ScrollController : MonoBehaviour
    {
        [FormerlySerializedAs("Control")]
        public ListPositionCtrl Control;

        [UICreated]
        private void Init(List<ShopItemPanel> shopItemPanels)
        {
            ListPositionCtrl control = gameObject.GetComponent<ListPositionCtrl>();
            Control = control;
            foreach (ShopItemPanel itemPanel in shopItemPanels) {
                control.listBoxes.Add(itemPanel.GetComponent<ListBox>());
            }
        }
    }
}