using System.Collections.Generic;
using System.Linq;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using Drone.Billing.Service;
using Drone.Core.Filter;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Event;
using Drone.LevelMap.Levels.IoC;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Repository;
using Drone.LevelMap.Levels.UI.LevelDiscription.DescriptionLevelDialog;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;

namespace Drone.LevelMap.Levels.Service
{
    public class LevelService : GameEventDispatcher, IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private ProgressRepository _progressRepository;

        [Inject]
        private LevelDescriptorRegistry _levelDescriptorRegistry;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private BillingService _billingService;

        private List<LevelViewModel> _levelsViewModels = new List<LevelViewModel>();
        public string SelectedLevelId { get; set; }
        public string SelectedDroneId { get; set; }

        public void Init()
        {
            _resourceService.LoadConfiguration("Configs/levels@embeded", LoadLevelsDescriptors);
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
        public string GetNextLevelId(string levelId)
        {
            LevelDescriptor levelDescriptor = _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Id == levelId);
            LevelDescriptor nextLevel = _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Order == levelDescriptor.Order + 1);
            return nextLevel != null ? _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Order == levelDescriptor.Order + 1).Id : null;
        }

        public int GetChipsCount(string levelId)
        {
            return GetLevelProgressById(levelId).CountChips;
        }

        public int GetStarsCount(string levelId)
        {
            return GetLevelProgressById(levelId).CountStars;
        }

        public int GetIntZoneId(string regionId)
        {
            int.TryParse(string.Join("", regionId.Where(char.IsDigit)), out int value);
            return value;
        }

        public int GetCurrentLevel()
        {
            List<LevelDescriptor> descriptors = _levelDescriptorRegistry.LevelDescriptors.OrderBy(o => o.Order).ToList();
            foreach (LevelDescriptor descriptor in descriptors) {
                LevelProgress progress = GetPlayerProgressModel().LevelsProgress.FirstOrDefault(a => a.Id == descriptor.Id);
                if (progress == null) {
                    return descriptor.Order;
                }
            }
            return 0;
        }

        public float GetTransitTime(string levelId)
        {
            return GetLevelProgressById(levelId).TransitTime;
        }

        public List<LevelViewModel> GetLevels()
        {
            _levelsViewModels = new List<LevelViewModel>();
            PlayerProgressModel playerProgressModel = GetPlayerProgressModel();
            foreach (LevelDescriptor descriptor in _levelDescriptorRegistry.LevelDescriptors) {
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
            return _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Id == levelId);
        }

        public LevelProgress GetLevelProgressById(string levelId)
        {
            PlayerProgressModel playerModel = GetPlayerProgressModel();
            LevelProgress level = playerModel.LevelsProgress.Find(x => x.Id.Equals(levelId));
            return level;
        }

        public PlayerProgressModel GetPlayerProgressModel()
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

        private void LoadLevelsDescriptors(Configuration config, object[] loadparameters)
        {
            foreach (Configuration configuration in config.GetList<Configuration>("levels.level")) {
                LevelDescriptor levelDescriptor = new LevelDescriptor();
                levelDescriptor.Configure(configuration);
                _levelDescriptorRegistry.LevelDescriptors.Add(levelDescriptor);
            }
        }
    }
}