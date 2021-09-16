using System.Collections.Generic;
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
    public class LevelsMapController : MonoBehaviour
    {
        [Inject]
        private LevelService _levelService;

        [Inject]
        private UIService _uiService;

        private List<LevelViewModel> _levelViewModels;

        private List<RegionDescriptor> _regionDescriptors;

        private List<ProgressMapItemController> progressMapItemController = new List<ProgressMapItemController>();

        [UICreated]
        private void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
            CreateRegions();
        }

        private void OnDestroy()
        {
            _levelService.RemoveListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
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
                if (_levelService.GetIntRegionId(regionDescriptor.Id) > _levelService.GetIntRegionId(_levelService.GetCurrentRegionId())) {
                    SetRegionActivity(regionDescriptor.Id, true);
                    continue;
                }
                CreateLevels(regionDescriptor, _levelViewModels);
            }
            UpdateRegions();
        }

        private void CreateLevels(RegionDescriptor regionDescriptor, List<LevelViewModel> viewModels)
        {
            foreach (string levelId in regionDescriptor.LevelId) {
                LevelViewModel levelViewModel = viewModels.Find(x => x.LevelDescriptor.Id.Equals(levelId));
                GameObject levelContainer = GameObject.Find($"level{levelViewModel.LevelDescriptor.Order}");
                _uiService.Create<ProgressMapItemController>(UiModel.Create<ProgressMapItemController>(levelViewModel,
                                                                        levelViewModel.LevelDescriptor.Order == _levelService.GetNextLevel(),
                                                                        levelViewModel.LevelDescriptor.Type == LevelType.BOSS)
                                                                    .Container(levelContainer))
                          .Then(controller => progressMapItemController.Add(controller))
                          .Done();
            }
        }

        private void UpdateRegions()
        {
            PlayerProgressModel model = _levelService.GetPlayerProgressModel();
            RegionDescriptor regionDescriptor = _levelService.GetRegionDescriptorById(model.CurrentRegionId);
            RegionDescriptor nextRegion = _regionDescriptors.Find(x => x.Id.Equals(_levelService.GetNextRegionId(regionDescriptor.Id)));
            _levelViewModels = _levelService.GetLevels();
            UpdateLevels(_levelViewModels);

            if (nextRegion == null) {
                return;
            }

            if (model.LevelsProgress.Count == 0) {
                SetRegionActivity(nextRegion.Id, true);
            }

            if (!_levelService.CompletedRegionConditions(regionDescriptor.Id, nextRegion.CountStars)) {
                return;
            }

            SetRegionActivity(nextRegion.Id, false);
            CreateLevels(nextRegion, _levelViewModels);
            _levelService.SaveCurrentRegionId(nextRegion.Id);
        }

        private void UpdateLevels(List<LevelViewModel> levelViewModels)
        {
            foreach (ProgressMapItemController spotController in progressMapItemController) {
                LevelDescriptor descriptor = spotController.LevelViewModel.LevelDescriptor;
                LevelViewModel model = levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(descriptor.Id));
                spotController.UpdateSpot(model, descriptor.Order == _levelService.GetNextLevel());
            }
        }

        private void SetRegionActivity(string regionId, bool value)
        {
            GameObject regionContainer = GameObject.Find(regionId);
            regionContainer.GetChildren().Find(x => x.name == "Fog").SetActive(value);
            regionContainer.GetChildren().Find(x => x.name == "PlateWithDescription").SetActive(value);
        }
    }
}