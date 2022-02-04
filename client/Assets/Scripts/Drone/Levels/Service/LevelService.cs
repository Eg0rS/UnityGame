using System.Collections.Generic;
using System.Linq;
using AgkCommons.Event;
using AgkUI.Dialog.Service;
using Drone.Billing.Service;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.LevelMap.UI.DescriptionLevelDialog;
using Drone.Levels.Descriptor;
using Drone.Levels.Event;
using Drone.Levels.Model;
using Drone.Levels.Repository;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using RSG.Promises;

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

        public void SetLevelProgress(string levelId, Dictionary<LevelTask, bool> levelTasks, int countChips)
        {
            PlayerProgressModel model = GetPlayerProgressModel();
            LevelProgress levelProgress = model.LevelsProgress.FirstOrDefault(a => a.Id == levelId);
            if (levelProgress == null) {
                levelProgress = new LevelProgress() {
                        Id = levelId,
                        LevelTasks = new Dictionary<string, bool>()
                };
                model.LevelsProgress.Add(levelProgress);
            }
            foreach (KeyValuePair<LevelTask, bool> task in levelTasks) {
                if (levelProgress.LevelTasks.ContainsKey(task.Key.Description)) {
                    levelProgress.LevelTasks[task.Key.Description] = task.Value ? task.Value : levelProgress.LevelTasks[task.Key.Description];
                } else {
                    levelProgress.LevelTasks[task.Key.Description] = task.Value;
                }
            }
            int countTasks = levelProgress.LevelTasks.Count;
            int doneTasks = levelProgress.LevelTasks.Count(x => x.Value);
            float doneProcent = doneTasks / countTasks;
            if (doneProcent <= 0.40f) {
                levelProgress.CountStars = 1;
            } else if (doneProcent > 0.40f && doneProcent <= 0.75f) {
                levelProgress.CountStars = 2;
            } else if (doneProcent > 0.75f) {
                levelProgress.CountStars = 3;
            }
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