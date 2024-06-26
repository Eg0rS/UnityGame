﻿using System.Collections.Generic;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.Levels.Descriptor;
using Drone.Levels.Model;
using Drone.MainMenu.UI.Panel;
using IoC.Attribute;
using UnityEngine;

namespace Drone.LevelMap.UI
{
    [UIController("UI_Prototype/Button/pfLevelButton@embeded")]
    public class ProgressMapItemController : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();

        [Inject]
        private DialogManager _dialogManager;

        [UIComponentBinding("Star1")]
        private ToggleButton _star1;
        [UIComponentBinding("Star2")]
        private ToggleButton _star2;
        [UIComponentBinding("Star3")]
        private ToggleButton _star3;

        [UIComponentBinding("LevelLabel")]
        private UILabel _label;

        [UIComponentBinding]
        private UIButton _button;
        [UIObjectBinding("Open")]
        private GameObject _open;
        [UIObjectBinding("Close")]
        private GameObject _close;

        private List<ToggleButton> _stars1;

        [UIObjectBinding("Progress")]
        private GameObject _progress;

        private LevelViewModel _levelViewModel;

        public LevelViewModel LevelViewModel
        {
            get { return _levelViewModel; }
        }

        [UICreated]
        private void Init(LevelViewModel levelViewModel, bool isCurrent)
        {
            _stars1 = new List<ToggleButton>() {
                    _star1,
                    _star2,
                    _star3
            };
            _button.onClick.AddListener(Click);
            UpdateSpot(levelViewModel, isCurrent);
        }

        public void UpdateSpot(LevelViewModel levelViewModel, bool isCurrent)
        {
            _levelViewModel = levelViewModel;
            DisableSpotProgress();
            if (_levelViewModel.LevelDescriptor.Type == LevelType.NONE || (_levelViewModel.LevelProgress == null && !isCurrent)) {
                _close.SetActive(true);
                _button.interactable = false;
                return;
            }
            _open.SetActive(true);
            _label.text = _levelViewModel.LevelDescriptor.Order.ToString();
            _button.interactable = true;
            if (isCurrent) {
                _button.Select();
            }
            SetCompletedSpot();
        }

        private void SetCompletedSpot()
        {
            _progress.SetActive(true);
            foreach (ToggleButton star in _stars1) {
                star.Interactable = false;
                if (_levelViewModel.LevelProgress != null) {
                    star.IsOn = true;
                    continue;
                }
                star.IsOn = false;
            }
        }

        private void DisableSpotProgress()
        {
            _open.SetActive(false);
            _close.SetActive(false);
            _progress.SetActive(false);
        }

        private void Click()
        {
            if (!_button.interactable) {
                return;
            }
            _dialogManager.Show<DescriptionLevelDialog.DescriptionLevelDialog>(_levelViewModel);
            _logger.Debug("start dialog: " + _levelViewModel.LevelDescriptor.Id);
        }
    }
}