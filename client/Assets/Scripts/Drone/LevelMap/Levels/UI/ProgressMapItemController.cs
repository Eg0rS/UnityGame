using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Element.Text;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Service;
using Drone.MainMenu.UI.Panel;
using IoC.Attribute;
using UnityEngine;

namespace Drone.LevelMap.Levels.UI
{
    [UIController("UI/Panel/pfLevelProgressItemPanel@embeded")]
    public class ProgressMapItemController : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();

        [Inject]
        private LevelService _levelService;
        [UIObjectBinding("Stars")]
        private GameObject _stars;

        [UIObjectBinding("StageCurrent")]
        private GameObject _stageCurrent;

        [UIObjectBinding("StageCompleted")]
        private GameObject _stageCompleted;

        [UIObjectBinding("StageNotOpen")]
        private GameObject _stageNotOpen;

        [UIObjectBinding("StageLock")]
        private GameObject _stageLock;

        [UIObjectBinding("Order")]
        private GameObject _order;

        [UIObjectBinding("Star1")]
        private GameObject _oneStar;

        [UIObjectBinding("Star2")]
        private GameObject _twoStar;

        [UIObjectBinding("Star3")]
        private GameObject _threeStar;

        private UILabel _orderLabel;
        private LevelViewModel _levelViewModel;
        private bool _isCanClick;

        public LevelViewModel LevelViewModel
        {
            get { return _levelViewModel; }
        }

        [UICreated]
        private void Init(LevelViewModel levelViewModel, bool isCurrent)
        {
            _orderLabel = _order.GetComponent<UILabel>();
            UpdateSpot(levelViewModel, isCurrent);
        }

        public void UpdateSpot(LevelViewModel levelViewModel, bool isCurrent)
        {
            _levelViewModel = levelViewModel;
            DisableSpotProgress();
            if (_levelViewModel.LevelDescriptor.Prefab == "") {
                SetLockedSpot();
            } else if (_levelViewModel.LevelProgress != null) {
                SetCompletedSpot();
            } else if (_levelViewModel.LevelProgress == null && isCurrent) {
                SetCurrentSpot();
            } else {
                SetNotOpenSpot();
            }
        }

        private void SetLockedSpot()
        {
            _stageLock.SetActive(true);
            _stageNotOpen.SetActive(true);
        }

        private void SetCompletedSpot()
        {
            _stageCompleted.SetActive(true);
            _stars.SetActive(true);
            if (_levelViewModel.LevelProgress.CountStars == 1) {
                _oneStar.SetActive(true);
            } else if (_levelViewModel.LevelProgress.CountStars == 2) {
                _twoStar.SetActive(true);
            } else if (_levelViewModel.LevelProgress.CountStars == 3) {
                _threeStar.SetActive(true);
            }
            SetOrder();
            _isCanClick = true;
        }

        private void SetCurrentSpot()
        {
            _stageCurrent.SetActive(true);
            SetOrder();
            _orderLabel.color = Color.black;
            _isCanClick = true;
        }

        private void SetNotOpenSpot()
        {
            _stageNotOpen.SetActive(true);
            SetOrder();
            _isCanClick = false;
        }

        private void DisableSpotProgress()
        {
            _stars.SetActive(false);
            _oneStar.SetActive(false);
            _twoStar.SetActive(false);
            _threeStar.SetActive(false);
            _stageLock.SetActive(false);
            _stageNotOpen.SetActive(false);
            _stageCompleted.SetActive(false);
            _stageCurrent.SetActive(false);
            _order.SetActive(false);
            _orderLabel.color = Color.white;
        }

        private void SetOrder()
        {
            _order.SetActive(true);
            _orderLabel.text = _levelViewModel.LevelDescriptor.Order.ToString();
        }

        [UIOnClick("pfLocationItemSpot")]
        private void OnSpotClick()
        {
            if (!_isCanClick) {
                return;
            }
            _levelService.ShowStartLevelDialog(_levelViewModel.LevelDescriptor.Id);
            _logger.Debug("start dialog: " + _levelViewModel.LevelDescriptor.Id);
        }
    }
}