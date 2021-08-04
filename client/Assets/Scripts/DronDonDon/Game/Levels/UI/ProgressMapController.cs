using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Service;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using UnityEngine;
using AgkUI.Core.Model;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Event;
using DronDonDon.MainMenu.UI.Panel;
using NUnit.Framework;

namespace DronDonDon.Game.Levels.UI
{
    [UIController("UI/Panel/pfMiddlePanel@embeded")]
    public class ProgressMapController : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        [Inject]
        private LevelService _levelService;
        
        [Inject] 
        private UIService _uiService;
        
        private List<LevelViewModel> _levelViewModels;

        public List<ProgressMapItemController> _progressMapItemController = new List<ProgressMapItemController>();
        
        [UICreated]
        public void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
             CreateSpots();
        }
        
        private void OnLevelMapUpdated(LevelEvent levelEvent)
        {
            UpdateSpots();
        }

        private void CreateSpots()
        {
            _levelViewModels = _levelService.GetLevels();
            PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
            foreach (LevelViewModel item in _levelViewModels)
            {
                GameObject levelContainer = GameObject.Find($"level{item.LevelDescriptor.Order}");
                _uiService.Create<ProgressMapItemController>(UiModel
                            .Create<ProgressMapItemController>(item, item.LevelDescriptor.Id == playerProgressModel.CurrentLevel)
                            .Container(levelContainer))
                        .Then(controller => _progressMapItemController.Add(controller))
                        .Done();
            }
        }

        private void UpdateSpots()
        {
            _logger.Debug("update");
            _levelViewModels = _levelService.GetLevels();
            PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
            foreach (ProgressMapItemController spotController in _progressMapItemController)
            {
                LevelDescriptor descriptor = spotController.LevelViewModel.LevelDescriptor;
                
                LevelViewModel model = _levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(descriptor.Id));
                spotController.UpdateSpot(model, descriptor.Id == playerProgressModel.CurrentLevel);
            }
        }
    }
}