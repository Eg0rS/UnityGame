using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using DronDonDon.MainMenu.UI.Panel;
using IoC.Attribute;
using UnityEngine;
using LocationService = DronDonDon.Location.Service.LocationService;

namespace DronDonDon.Game.Levels.UI
{
    [UIController("UI/Panel/pfLevelProgressItemPanel@embeded")]
    public class ProgressMapItemController : MonoBehaviour
    {
        private const string COMPLETED_NAME_IMAGE = "Сompleted";
        private const string NEXT_NAME_IMAGE = "Next";
        
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        [Inject] 
        private LevelService _levelService;

        [Inject] 
        private LocationService _locationService;
        
        [UIObjectBinding("Stars")] 
        private GameObject _stars;
        
        [UIObjectBinding("Сompleted")] 
        private GameObject _completedLevelImage;
        
        [UIObjectBinding("Next")] 
        private GameObject _nextLevelImage;
        
        [UIObjectBinding("Locked")] 
        private GameObject _lockedLevelImage;

        [UIObjectBinding("pfLocationItemSpot")] 
        private GameObject _spot;
        
        [UIObjectBinding("LevelNumber")] 
        private GameObject _levelNumber;

        [UIObjectBinding("OneStar")] 
        private GameObject _firstStar;
        
        [UIObjectBinding("TwoStar")] 
        private GameObject _secondStar;
        
        [UIObjectBinding("ThreeStar")] 
        private GameObject _thirdStar;

        public LevelViewModel _levelViewModel;
        
        
        [UICreated]
        public void Init(LevelViewModel levelViewModel, bool isCurrentLevel)
        {
            DisableStars();
            DisableProgressImages();
            _levelViewModel = levelViewModel;
            _levelNumber.GetComponent<UILabel>().text = levelViewModel.LevelDescriptor.Order.ToString();
            if (levelViewModel.LevelProgress == null)
            {
                _lockedLevelImage.SetActive(true);
            }
            else
            {
                _lockedLevelImage.SetActive(false);
                if (isCurrentLevel)
                {
                    _nextLevelImage.SetActive(true);
                    return;
                }
                _completedLevelImage.SetActive(true);
                SetStars(levelViewModel.LevelProgress.CountStars);
            }
        }

        [UIOnClick("pfLocationItemSpot")]
        private void SelectLevel()
        {
            if (_levelViewModel.LevelProgress != null)
            {
               _logger.Debug("start dialog: "+ _levelViewModel.LevelDescriptor.Id); 
            }
        }

        private List<GameObject> GetStarsImage()
        {
            List<GameObject> stars = _stars.GetChildren();
            return stars;
        }

        private void SetStars(int countStars)
        {
            List<GameObject> stars = GetStarsImage();
            for (int i = 0; i < countStars; i++)
            {
                stars[i].SetActive(true);
            }
        }

        private void DisableStars()
        {
            _firstStar.SetActive(false);
            _secondStar.SetActive(false);
            _thirdStar.SetActive(false);
        }

        private void DisableProgressImages()
        {
            _completedLevelImage.SetActive(false);
            _nextLevelImage.SetActive(false);
        }

        public void UpdateSpot(LevelViewModel levelViewModel, bool isCurrentLevel)
        {
            DisableStars();
            DisableProgressImages();
            _levelNumber.GetComponent<UILabel>().text = levelViewModel.LevelDescriptor.Order.ToString();
            if (levelViewModel.LevelProgress == null)
            {
                _lockedLevelImage.SetActive(true);
            }
            else
            {
                _lockedLevelImage.SetActive(false);
                if (isCurrentLevel)
                {
                    _nextLevelImage.SetActive(true);
                    return;
                }
                _completedLevelImage.SetActive(true);
                SetStars(levelViewModel.LevelProgress.CountStars);
            }
        }
    }
}