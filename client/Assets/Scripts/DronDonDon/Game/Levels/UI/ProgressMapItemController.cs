using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
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
        
        [Inject] 
        private LevelService _levelService;

        [Inject]
        private UIService _uiService;

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
        
        [UIObjectBinding("Selected")] 
        private GameObject _selectedLevel;
        
        [UIObjectBinding("OneStar")] 
        private GameObject _firstStar;
        
        [UIObjectBinding("TwoStar")] 
        private GameObject _secondStar;
        
        [UIObjectBinding("ThreeStar")] 
        private GameObject _thirdStar;

        private LevelViewModel _levelViewModel;


        [UICreated]
        public void Init(LevelViewModel levelViewModel, bool isCurrentLevel)
        {
            ProgressMapController._progressMapItemController.Add(this);
            DisableStars();
            DisablePorgressImages();
            _levelViewModel = levelViewModel;
            if (levelViewModel.LevelProgress == null)
            {
                _levelNumber.GetComponent<UILabel>().text = levelViewModel.LevelDescriptor.Order.ToString();
                _lockedLevelImage.SetActive(true);
            }
            else
            {
                _levelNumber.GetComponent<UILabel>().text = levelViewModel.LevelDescriptor.Order.ToString();
                if (isCurrentLevel)
                {
                    ProgressMapController.AddSelectedLevel(_selectedLevel);
                    _selectedLevel.SetActive(true);
                    _locationService.NameSelectedLevel = levelViewModel.LevelDescriptor.Prefab;
                    _nextLevelImage.SetActive(true);
                    _lockedLevelImage.SetActive(false);
                    return;
                }
                _completedLevelImage.SetActive(true);
                _lockedLevelImage.SetActive(false);
                SetStars(levelViewModel.LevelProgress.CountStars);
            }
        }

        [UIOnClick("pfLocationItemSpot")]
        private void SelectLevel()
        {
            PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
            string levelId = transform.parent.name;
            if (playerProgressModel.LevelsProgress.Find(x => x.Id.Equals(levelId)).TransitTime != 0 )
            {
                _locationService.NameSelectedLevel = _levelViewModel.LevelDescriptor.Prefab;
                _locationService.StartGame();
            }
            // if ()
            // {
            //     ProgressMapController.UnEnableSelectedLevel();
            //     _selectedLevel.SetActive(true);
            //     ProgressMapController.AddSelectedLevel(_selectedLevel);
            //     _locationService.NameSelectedLevel = _levelViewModel.LevelDescriptor.Prefab;
            // }
        }
        
        private List<GameObject> GetSpotChildren()
        {
            List<GameObject> spotChildren = _spot.GetChildren();
            return spotChildren;
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

        private void DisablePorgressImages()
        {
            _completedLevelImage.SetActive(false);
            _nextLevelImage.SetActive(false);
        }
    }
}