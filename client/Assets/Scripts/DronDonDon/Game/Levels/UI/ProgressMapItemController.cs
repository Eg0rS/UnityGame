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

        [UIObjectBinding("pfLocationItemSpot")] 
        private GameObject _spot;
        
        [UIObjectBinding("LevelNumber")] 
        private GameObject _levelNumber;
        
        [UIObjectBinding("Selected")] 
        private GameObject _selectedLevel;

        private LevelDescriptor _levelDescriptor;
        private bool _isCurrentLevel;
        private bool _isCompleted;

        [UICreated]
        public void Init(LevelProgress levelProgress, LevelDescriptor levelDescriptor, bool isCurrentLevel = false)
        {
            _levelDescriptor = levelDescriptor;
            _isCurrentLevel = isCurrentLevel;
            if (levelProgress.CountStars != 0)
            {
                _isCompleted = true;
                SetProgressImage(COMPLETED_NAME_IMAGE);
                _levelNumber.GetComponent<UILabel>().text = levelDescriptor.Order.ToString();
                SetStars(levelProgress.CountStars);
                return;
            }
            if (isCurrentLevel)
            {
                _selectedLevel.SetActive(true);
                _locationService.NameSelectedLevel = _levelDescriptor.Prefab;
                SetProgressImage(NEXT_NAME_IMAGE);
                _levelNumber.GetComponent<UILabel>().text = levelDescriptor.Order.ToString();
            }
            else
            {
                _levelNumber.GetComponent<UILabel>().text = levelDescriptor.Order.ToString();
            }
        }

        [UIOnClick("pfLocationItemSpot")]
        private void SelectLevel()
        {
            if (_isCurrentLevel || _isCompleted)
            {
                ProgressMapController.UnEnableSelectedLevel(_selectedLevel);
                _selectedLevel.SetActive(true);
                ProgressMapController.AddSelectedLevel(_selectedLevel);
                _locationService.NameSelectedLevel = _levelDescriptor.Prefab;
            }
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

        private void SetProgressImage(string nameImage)
        {
            List<GameObject> spotChildren = GetSpotChildren();
            switch (nameImage)
            {
                case "Сompleted":
                    spotChildren[2].SetActive(false);
                    spotChildren[0].SetActive(true);
                    break;
                case "Next":
                    spotChildren[2].SetActive(false);
                    spotChildren[1].SetActive(true);
                    break;
            }
        }
    }
}