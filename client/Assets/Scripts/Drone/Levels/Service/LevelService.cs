using System.Collections.Generic;
using System.Linq;
using AgkCommons.Event;
using Drone.Billing.Service;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Levels.Event;
using Drone.Levels.Model;
using Drone.Levels.Repository;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.Levels.Service
{
    public class LevelService : GameEventDispatcher, IConfigurable
    {
        [Inject]
        private ProgressRepository _progressRepository;
        [Inject]
        private BillingService _billingService;
        [Inject]
        private LevelsDescriptors _levelsDescriptors;

        private List<LevelViewModel> _levelsViewModels = new List<LevelViewModel>();
        public string SelectedLevelId { get; set; }
        public string SelectedDroneId { get; set; }

        public void SetLevelProgress(LevelDescriptor levelDescriptor, int countChips)
        {
            PlayerProgressModel playerProgress = GetPlayerProgressModel();
            LevelProgress levelProgress = new LevelProgress() {
                    Id = levelDescriptor.Id,
                    LevelVersion = levelDescriptor.Version,
                    CountChips = countChips
            };
            playerProgress.LevelsProgress.Add(levelProgress);
            _billingService.AddCredits(countChips);
            SaveProgress(playerProgress);
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
            LevelDescriptor nextLevel = _levelsDescriptors.Levels.ToList().FirstOrDefault(x => x.Order == levelDescriptor.Order + 1);
            return nextLevel;
        }

        public int GetCurrentLevel()
        {
            List<LevelDescriptor> descriptors = _levelsDescriptors.Levels.OrderBy(o => o.Order).ToList();
            PlayerProgressModel playerProgress = GetPlayerProgressModel();
            foreach (LevelDescriptor descriptor in descriptors) {
                LevelProgress progress = playerProgress.LevelsProgress.FirstOrDefault(a => a.Id == descriptor.Id);
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

        public void Configure()
        {
            if (!HasPlayerProgress()) {
                _progressRepository.Set(new PlayerProgressModel());
            }
        }
    }
}