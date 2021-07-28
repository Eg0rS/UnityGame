using System.Collections.Generic;
using System.Linq;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using IoC.Extension;
using NUnit.Framework;
using UnityEngine;
using AgkUI.Core.Model;

namespace DronDonDon.Game.Levels.UI
{
    [UIController("UI/Panel/pfLevelProgressItemPanel@embeded")]
    public class ProgressMapItemController : MonoBehaviour
    {
        [Inject] 
        private LevelService _levelService;

        [Inject]
        private UIService _uiService;

        // [UIObjectBinding("Chips")] 
        // private GameObject _chips;
        
        [UIObjectBinding("Stars")] 
        private GameObject _stars;
        
        // [UIObjectBinding("Time")] 
        // private GameObject _time;

        [UIObjectBinding("LevelNumber")] 
        private GameObject _levelNumber;

        [UICreated]
        public void Init(LevelProgress levelProgress)
        {
            if (levelProgress.CountStars != 0)
            {
                // _chips.GetComponent<UILabel>().text += levelProgress.CountChips;
                // _time.GetComponent<UILabel>().text += levelProgress.TransitTime;
                _levelNumber.GetComponent<UILabel>().text = levelProgress.Id;
                int countStars = levelProgress.CountStars;
                List<GameObject> starsChildren = _stars.GetChildren();
                switch (countStars)
                {
                    case 1:
                        starsChildren[0].SetActive(true);
                        break;
                    case 2:
                        starsChildren[0].SetActive(true);
                        starsChildren[1].SetActive(true);
                        break;
                    case 3:
                        starsChildren[0].SetActive(true);
                        starsChildren[1].SetActive(true);
                        starsChildren[2].SetActive(true);
                        break;
                }
            }
            else
            {
                SetActiveProgressPanel(false);
            }
        }

        public void SetActiveProgressPanel(bool value)
        {
            GameObject panel = gameObject.GetChildRecursive("Panel");
            panel.SetActive(value);
        }

        private List<GameObject> GetLevelsStateObjects()
        {
            GameObject starsContainer = gameObject.GetChildRecursive("pfLocationItemSpot");
            List<GameObject> result = new List<GameObject>(starsContainer.transform.childCount);
            for (int i = 0; i < result.Capacity; i++)
            {
                result.Add(starsContainer.transform.GetChild(i).gameObject);
            }
            return result;
        }
    }
}