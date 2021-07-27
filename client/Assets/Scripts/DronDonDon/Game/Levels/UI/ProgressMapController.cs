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
    [UIController("UI/Panel/pfMiddlePanel@embeded")]
    public class ProgressMapController : MonoBehaviour
    {
        //private string _pfLevelProgressItemPanel = "UI/Panel/pfLevelProgressItemPanel@embeded";
        //"Embeded/UI/Panel/pfLevelProgressItemPanel"
        [Inject] private LevelService _levelService;

        private List<LevelViewModel> _levelViewModels;

        [Inject]
        private UIService _uiService;

        [UICreated]
        public void Init()
        {
            // _levelService.SetChipsCount("1",100);
            // _levelService.SetStarsCount("1",3);
            // _levelService.SetTransitTime("1",3);
            //
            // _levelService.SetChipsCount("2",200);
            // _levelService.SetStarsCount("2",2);
            // _levelService.SetTransitTime("2",5);
            
            _levelService.DeletePlayerProgress();
            
            _levelViewModels = _levelService.GetLevels();
            CreateSpot();
        }

        private void CreateSpot()
        {
            for (int i = 0; i < _levelViewModels.Count; i++)
            {
                GameObject levelContainer = GameObject.Find($"level{i+1}");
                _uiService.Create<ProgressMapItemController>(UiModel.Create<ProgressMapItemController>(_levelViewModels[i].LevelProgress).Container(levelContainer)).Done();
                
                //GameObject progressPanel = levelContainer.GetChildRecursive("pfLevelProgressItemPanel(Clone)");
                // GameObject levelContainerChilds = levelContainer.GetChildRecursive("pfLevelProgressItemPanel(Clone)");
                // if (levelContainerChilds != null)
                // {
                //     Debug.Log("yess!!");
                //     Debug.Log(levelContainer.name);
                // }
                // List<GameObject> panelChilds = progressPanel.GetChildren();
                //
                // GameObject chips = panelChilds.First(x => x.name == "Chips");
                // chips.gameObject.GetComponent<UILabel>().text = _levelViewModels[i].LevelProgress.CountChips.ToString();
                //
                // GameObject time = panelChilds.First(x => x.name == "Time");
                // time.gameObject.GetComponent<UILabel>().text = _levelViewModels[i].LevelProgress.TransitTime.ToString();
                //
                // GameObject stars = panelChilds.First(x => x.name == "Stars");
                //
                // List<GameObject> starsChild = stars.GetChildren();
                // switch (_levelViewModels[i].LevelProgress.CountStars)
                // {
                //     case 1:
                //         starsChild[1].SetActive(true);
                //         break;
                //     case 2:
                //         starsChild[1].SetActive(true);
                //         starsChild[2].SetActive(true);
                //         break;
                //     case 3:
                //         starsChild[1].SetActive(true);
                //         starsChild[2].SetActive(true);
                //         starsChild[3].SetActive(true);
                //         break;
                }
            }
        }
    }