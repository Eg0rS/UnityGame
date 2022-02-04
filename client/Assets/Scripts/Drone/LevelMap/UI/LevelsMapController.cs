using System.Collections.Generic;
using System.Linq;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using Drone.Levels.Descriptor;
using Drone.Levels.Event;
using Drone.Levels.Model;
using Drone.Levels.Service;
using IoC.Attribute;
using UnityEngine;

namespace Drone.LevelMap.UI
{
    [UIController("UI_prototype/Panel/LevelMap/pfLevelMapPanel@embeded")]
    public class LevelsMapController : MonoBehaviour
    {
        [Inject]
        private LevelService _levelService;

        [Inject]
        private UIService _uiService;

        private List<ProgressMapItemController> _progressMapItemController = new List<ProgressMapItemController>();

        private string _currentLevelId;

        [UIObjectBinding("Content")]
        private GameObject _levelsContainer;

        [UICreated]
        private void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);

            CreateLevels(_levelService.GetLevels());
        }

        private void OnDestroy()
        {
            _levelService.RemoveListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
        }

        private void OnLevelMapUpdated(LevelEvent levelEvent)
        {
            UpdateLevels(_levelService.GetLevels());
        }

        private void CreateLevels(List<LevelViewModel> levelViewModels)
        {
            LevelViewModel ViewModel = levelViewModels.FirstOrDefault(x => x.LevelDescriptor.Order.Equals(_levelService.GetCurrentLevel()));
            _currentLevelId = null;
            if (ViewModel != null) {
                _currentLevelId = ViewModel.LevelDescriptor.Id;
            }
            foreach (LevelViewModel levelViewModel in levelViewModels) {
                _uiService.Create<ProgressMapItemController>(UiModel.Create<ProgressMapItemController>(levelViewModel,
                                                                        levelViewModel.LevelDescriptor.Id.Equals(_currentLevelId))
                                                                    .Container(_levelsContainer))
                          .Then(controller => _progressMapItemController.Add(controller))
                          .Done();
            }
        }

        private void UpdateLevels(List<LevelViewModel> levelViewModels)
        {
            _currentLevelId = levelViewModels.Find(x => x.LevelDescriptor.Order.Equals(_levelService.GetCurrentLevel())).LevelDescriptor.Id;
            foreach (ProgressMapItemController spotController in _progressMapItemController) {
                LevelDescriptor descriptor = spotController.LevelViewModel.LevelDescriptor;
                LevelViewModel model = levelViewModels.Find(x => x.LevelDescriptor.Id.Equals(descriptor.Id));
                spotController.UpdateSpot(model, model.LevelDescriptor.Id.Equals(_currentLevelId));
            }
        }

        public string CurrentLevelId
        {
            get { return _currentLevelId; }
            set { _currentLevelId = value; }
        }
    }
}