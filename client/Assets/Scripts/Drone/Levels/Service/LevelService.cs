using System.Collections.Generic;
using System.Linq;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Billing.Service;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.UI.LevelDiscription.DescriptionLevelDialog;
using Drone.Levels.Descriptor;
using Drone.Levels.Event;
using Drone.Levels.Model;
using Drone.Levels.Repository;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;

namespace Drone.Levels.Service
{
    public class LevelService : GameEventDispatcher, IConfigurable
    {
        [Inject]
        private ProgressRepository _progressRepository;
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        [Inject]
        private BillingService _billingService;
        [Inject]
        private LevelsDescriptors _levelsDescriptors;

        private List<LevelViewModel> _levelsViewModels = new List<LevelViewModel>();
        public string SelectedLevelId { get; set; }
        public string SelectedDroneId { get; set; }

        public void Configure()
        {
        }

        public void ShowStartLevelDialog(string leveId)
        {
            LevelDescriptor levelDescriptor = _levelsViewModels.Find(x => x.LevelDescriptor.Id.Equals(leveId)).LevelDescriptor;
            _dialogManager.Require().Show<DescriptionLevelDialog>(levelDescriptor);
        }

        public void SetLevelProgress(string levelId, int countStars, int countChips, float transitTime, int durability)
        {
            PlayerProgressModel model = GetPlayerProgressModel();
            LevelProgress levelProgress = model.LevelsProgress.FirstOrDefault(a => a.Id == levelId);
            if (levelProgress == null) {
                levelProgress = new LevelProgress() {
                        Id = levelId
                };
                model.LevelsProgress.Add(levelProgress);
            }
            levelProgress.CountChips = countChips;
            if (levelProgress.CountStars < countStars) {
                levelProgress.CountStars = countStars;
            }
            levelProgress.TransitTime = transitTime;
            levelProgress.Durability = durability;
            _billingService.AddCredits(countChips);
            SaveProgress(model);
        }

        public void ResetPlayerProgress()
        {
            _progressRepository.Delete();
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
        }

        [CanBeNull]
        public LevelDescriptor GetNextLevelDescriptor(string levelId)
        {
            LevelDescriptor levelDescriptor = _levelsDescriptors.Levels.ToList().Find(x => x.Id == levelId);
            LevelDescriptor nextLevel = _levelsDescriptors.Levels.ToList().Find(x => x.Order == levelDescriptor.Order + 1);
            return nextLevel;
        }

        public int GetCurrentLevel()
        {
            List<LevelDescriptor> descriptors = _levelsDescriptors.Levels.OrderBy(o => o.Order).ToList();
            foreach (LevelDescriptor descriptor in descriptors) {
                LevelProgress progress = GetPlayerProgressModel().LevelsProgress.FirstOrDefault(a => a.Id == descriptor.Id);
                if (progress == null) {
                    return descriptor.Order;
                }
            }
            return 0;
        }

        public List<LevelViewModel> GetLevels()
        {
            _levelsViewModels = new List<LevelViewModel>();
            PlayerProgressModel playerProgressModel = GetPlayerProgressModel();
            foreach (LevelDescriptor descriptor in _levelsDescriptors.Levels.OrderBy(o => o.Order)) {
                LevelViewModel levelViewModel = new LevelViewModel {
                        LevelDescriptor = descriptor,
                        LevelProgress = playerProgressModel.LevelsProgress.Find(x => x.Id.Equals(descriptor.Id))
                };
                _levelsViewModels.Add(levelViewModel);
            }
            return _levelsViewModels;
        }

        public LevelDescriptor GetLevelDescriptorById(string levelId)
        {
            return _levelsDescriptors.Levels.ToList().Find(x => x.Id == levelId);
        }

        private PlayerProgressModel GetPlayerProgressModel()
        {
            return !HasPlayerProgress() ? new PlayerProgressModel() : _progressRepository.Require();
        }

        private bool HasPlayerProgress()
        {
            return _progressRepository.Exists();
        }

        private void SaveProgress(PlayerProgressModel model)
        {
            _progressRepository.Set(model);
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
        }

        public LevelsDescriptors LevelsDescriptors
        {
            get { return _levelsDescriptors; }
        }
    }
}