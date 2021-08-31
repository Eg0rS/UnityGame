using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using CircularScrollingList;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeliveryRush.LevelMap.Levels.UI.LevelDiscription.DescriptionLevelDialog
{
    [UIController("UI/Scrolls/pfScrollViewDroneStore@embeded")]
    public class ScrollControllerForDescriptionDialog : MonoBehaviour
    {
        [FormerlySerializedAs("Control")]
        public ListPositionCtrl _control;

        [UICreated]
        private void Init(List<ViewDronPanel> viewDronPanels)
        {
            ListPositionCtrl control = gameObject.GetComponent<ListPositionCtrl>();
            _control = control;
            foreach (ViewDronPanel itemPanel in viewDronPanels) {
                control.listBoxes.Add(itemPanel.GetComponent<ListBox>());
            }
        }
    }
}