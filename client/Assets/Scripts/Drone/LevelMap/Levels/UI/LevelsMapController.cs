using System;
using System.Collections.Generic;
using AgkCommons.Event;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Event;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Service;
using Drone.LevelMap.Regions.Descriptor;
using IoC.Attribute;
using UnityEngine;

namespace Drone.LevelMap.Levels.UI
{
    [UIController("UI/Panel/pfMiddlePanel@embeded")]
    public class LevelsMapController : GameEventDispatcher
    {
        [Inject]
        private LevelService _levelService;

        [Inject]
        private UIService _uiService;

        private List<LevelViewModel> _levelViewModels;

        private List<RegionDescriptor> _regionDescriptors;

        private List<ProgressMapItemController> progressMapItemController = new List<ProgressMapItemController>();

        [UICreated]
        public void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
            CreateRegions();
        }

        private void OnLevelMapUpdated(LevelEvent levelEvent)
        {
            UpdateRegions();
        }

        private void CreateRegions()
        {
            _levelViewModels = _levelService.GetLevels();
            _regionDescriptors = _levelService.GetRegionDescriptors();
            foreach (RegionDescriptor regionDescriptor in _regionDescriptors) {
                GameObject regionContainer = GameObject.Find($"Region{regionDescriptor.Id}");
                if (int.Parse(regionDescriptor.Id) > int.Parse(_levelService.GetCurrentRegionId())) {
                    regionContainer.GetChildren().Find(x => x.name == "Fog").SetActive(true);
                    regionContainer.GetChildren().Find(x => x.name == "PlateWithDescription").SetActive(true);
                    continue;
                }
                CreateLevels(regionDescriptor, _levelViewModels);
            }
        }

        private void UpdateRegions()
        {
            PlayerProgressModel model = _levelService.GetPlayerProgressModel();
            RegionDescriptor regionDescriptor = _levelService.GetRegionDescriptorById(model.CurrentRegionId);
            _levelViewModels = _levelService.GetLevels();
            UpdateLevels(_levelViewModels);
            if (_levelService.CalculateCountStarsRegion(regionDescriptor.Id) < regionDescriptor.CountStars
                || !_levelService.PassedBoss(regionDescriptor.Id)) {
                return;
            }
            RegionDescriptor nextRegion = _regionDescriptors.Find(x => x.Id.Equals(Convert.ToString(int.Parse(regionDescriptor.Id) + 1)));
            if (nextRegion == null) {
                return;
            }
            GameObject regionContainer = GameObject.Find($"Region{regionDescriptor.Id}");
            regionContainer.GetChildren().Find(x => x.name == "Fog").SetActive(false);
            regionContainer.GetChildren().Find(x => x.name == "PlateWithDescription").SetActive(false);
            CreateLevels(nextRegion, _levelViewModels);
            model.CurrentRegionId = nextRegion.Id;
            _levelService.SaveProgress(model);
        }

        private void UpdateLevels(List<LevelViewModel> levelViewModels)
        {
            foreach (ProgressMapItemController spotController in progressMapItemController) {
                LevelDescriptor descriptor = spotController.LevelViewModel.LevelDescriptor;
                LevelViewModel model = levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(descriptor.Id));
                spotController.UpdateSpot(model, descriptor.Order == _levelService.GetNextLevel());
            }
        }

        private void CreateLevels(RegionDescriptor regionDescriptor, List<LevelViewModel> levelViewModels)
        {
            foreach (string levelId in regionDescriptor.LevelId) {
                GameObject levelContainer = GameObject.Find($"level{levelId}");
                LevelViewModel levelViewModel = levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(levelContainer.name));
                _uiService.Create<ProgressMapItemController>(UiModel.Create<ProgressMapItemController>(levelViewModel,
                                                                        levelViewModel.LevelDescriptor.Order == _levelService.GetNextLevel(),
                                                                        levelViewModel.LevelDescriptor.Order % 5 == 0)
                                                                    .Container(levelContainer))
                          .Then(controller => progressMapItemController.Add(controller))
                          .Done();
            }
        }
    }
}