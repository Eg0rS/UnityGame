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
        [Inject] 
        private LevelService _levelService;
        
        [Inject] 
        private UIService _uiService;
        
        private List<LevelViewModel> _levelViewModels;
        
        [UICreated]
        public void Init()
        {
            _levelService.DeletePlayerProgress();
            _levelViewModels = _levelService.GetLevels();
            CreateSpots();
        }

        private void CreateSpots()
        {
            for (int i = 0; i < _levelViewModels.Count; i++)
            {
                GameObject levelContainer = GameObject.Find($"level{i + 1}");
                _uiService.Create<ProgressMapItemController>(UiModel.Create<ProgressMapItemController>(_levelViewModels[i].LevelProgress).Container(levelContainer)).Done();
            }
        }
    }
}