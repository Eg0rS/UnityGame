using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Event;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Service;
using Drone.LevelMap.Zones.Descriptor;
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

        private List<ZoneDescriptor> _zoneDescriptors;

        private List<ProgressMapItemController> progressMapItemController = new List<ProgressMapItemController>();

        private string _currentZoneId;

        [UICreated]
        private void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
            CreateZones();
        }

        private void OnDestroy()
        {
            _levelService.RemoveListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
        }

        private void OnLevelMapUpdated(LevelEvent levelEvent)
        {
            UpdateZones();
        }

        private void CreateZones()
        {
            _levelViewModels = _levelService.GetLevels();
            _zoneDescriptors = _levelService.GetZonesDescriptors();
            _currentZoneId = _levelService.GetCurrentZoneId();
            foreach (ZoneDescriptor zoneDescriptor in _zoneDescriptors) {
                if (_levelService.GetIntZoneId(zoneDescriptor.Id) > _levelService.GetIntZoneId(_currentZoneId)) {
                    SetZoneActivity(zoneDescriptor.Id, true);
                    continue;
                }
                CreateLevels(zoneDescriptor, _levelViewModels);
            }
            UpdateZones();
        }

        private void CreateLevels(ZoneDescriptor zoneDescriptor, List<LevelViewModel> viewModels)
        {
            foreach (string levelId in zoneDescriptor.LevelId) {
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

        private void UpdateZones()
        {
            _levelViewModels = _levelService.GetLevels();
            _currentZoneId = _levelService.GetCurrentZoneId();
            PlayerProgressModel model = _levelService.GetPlayerProgressModel();
            ZoneDescriptor zoneDescriptor = _levelService.GetZoneDescriptorById(_currentZoneId);
            ZoneDescriptor nextZone = _zoneDescriptors.Find(x => x.Id.Equals(_levelService.GetNextZoneId(zoneDescriptor.Id)));
            UpdateLevels(_levelViewModels);

            if (nextZone == null) {
                return;
            }

            if (model.LevelsProgress.Count == 0) {
                SetZoneActivity(nextZone.Id, true);
            }

            if (!_levelService.CompletedZoneConditions(zoneDescriptor.Id, nextZone.CountStars)) {
                return;
            }

            SetZoneActivity(nextZone.Id, false);
            CreateLevels(nextZone, _levelViewModels);
        }

        private void UpdateLevels(List<LevelViewModel> levelViewModels)
        {
            foreach (ProgressMapItemController spotController in progressMapItemController) {
                LevelDescriptor descriptor = spotController.LevelViewModel.LevelDescriptor;
                LevelViewModel model = levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(descriptor.Id));
                spotController.UpdateSpot(model, descriptor.Order == _levelService.GetNextLevel());
            }
        }

        private void SetZoneActivity(string zoneId, bool value)
        {
            GameObject zoneContainer = GameObject.Find(zoneId);
            zoneContainer.GetChildren().Find(x => x.name == "Fog").SetActive(value);
            zoneContainer.GetChildren().Find(x => x.name == "PlateWithDescription").SetActive(value);
        }
    }
}