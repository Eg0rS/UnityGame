﻿using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using UnityEngine;

namespace DronDonDon.Resource.UI.DescriptionLevelDialog
{
    [UIController("UI/Dialog/ScrollView@embeded")]
    public class ScrollControllerForDescriptionDialog : MonoBehaviour
    {
        public ListPositionCtrl Control;
        
        [UICreated]
        private void Init(List<ViewDronPanel> _viewDronPanels)
        {
            ListPositionCtrl control = gameObject.GetComponent<ListPositionCtrl>();
            Control = control;
            foreach (var itemPanel in _viewDronPanels)
            {
                control.listBoxes.Add(itemPanel.GetComponent<ListBox>());
            }
        }
    }
}