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
using Drone.LevelMap.Regions.Descriptor;
using Drone.LevelMap.Regions.IoC;
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
        private RegionDescriptorRegistry _regionDescriptorRegistry;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private BillingService _billingService;

        private List<LevelViewModel> _levelsViewModels = new List<LevelViewModel>();
        public string CurrentLevelId { get; set; }

        public void Init()
        {
            if (_levelDescriptorRegistry.LevelDescriptors.Count != 0 && _regionDescriptorRegistry.RegionDescriptors.Count != 0) {
                return;
            }
            _resourceService.LoadConfiguration("Configs/levels@embeded", OnConfigLoaded);
            _resourceService.LoadConfiguration("Configs/regions@embeded", LoadRegionsDescriptors);
        }

        public void ShowStartLevelDialog(string leveId)
        {
            LevelDescriptor levelDescriptor = _levelsViewModels.Find(x => x.LevelDescriptor.Id.Equals(leveId)).LevelDescriptor;
            _dialogManager.Require().Show<DescriptionLevelDialog>(levelDescriptor);
        }

        public int GetNextLevel()
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

        public string GetCurrentRegionId()
        {
            PlayerProgressModel model = GetPlayerProgressModel();
            return model.CurrentRegionId;
        }

        public int GetChipsCount(string levelId)
        {
            return GetLevelProgressById(levelId).CountChips;
        }

        public int GetStarsCount(string levelId)
        {
            return GetLevelProgressById(levelId).CountStars;
        }

        public float GetTransitTime(string levelId)
        {
            return GetLevelProgressById(levelId).TransitTime;
        }

        public List<LevelViewModel> GetLevels()
        {
            _levelsViewModels = new List<LevelViewModel>();
            PlayerProgressModel playerProgressModel = GetPlayerProgressModel();
            foreach (LevelDescriptor item in _levelDescriptorRegistry.LevelDescriptors) {
                LevelViewModel levelViewModel = new LevelViewModel();
                levelViewModel.LevelDescriptor = item;
                levelViewModel.LevelProgress = playerProgressModel.LevelsProgress.Find(x => x.Id.Equals(item.Id));
                _levelsViewModels.Add(levelViewModel);
            }
            return _levelsViewModels;
        }

        public List<RegionDescriptor> GetRegionDescriptors()
        {
            return _regionDescriptorRegistry.RegionDescriptors;
        }

        public RegionDescriptor GetRegionDescriptorById(string id)
        {
            return _regionDescriptorRegistry.RegionDescriptors.Find(x => x.Id.Equals(id));
        }
        
        [NotNull]
        public string GetNextRegionId(string regionId)
        {
            // int id = GetIntRegionId(regionId);
            // string result = $"region{++id}";
            return "region2";
        }
        
        

        public int CalculateCountStarsRegion(string regionId)
        {
            RegionDescriptor regionDescriptor = GetRegionDescriptorById(regionId);
            if (regionDescriptor == null) {
                return 0;
            }
            int result = 0;
            foreach (string levelId in regionDescriptor.LevelId) {
                LevelProgress levelProgress = GetLevelProgressById($"level{levelId}");
                if (levelProgress != null) {
                    result += GetLevelProgressById($"level{levelId}").CountStars;
                }
            }
            return result;
        }

        public bool PassedBoss(string regionId)
        {
            PlayerProgressModel model = GetPlayerProgressModel();
            RegionDescriptor regionDescriptor = GetRegionDescriptorById(regionId);
            foreach (string levelId in regionDescriptor.LevelId) {
                if (_levelDescriptorRegistry.LevelDescriptors.Find(x => x.Id.Equals($"level{levelId}")).Type != LevelType.Boss) {
                    continue;
                }
                LevelProgress levelProgress = model.LevelsProgress.Find(x => x.Id.Equals($"level{levelId}"));
                return levelProgress != null;
            }
            return false;
        }
        
        public int GetIntRegionId(string regionId)
        {
            int.TryParse(string.Join("", regionId.Where(char.IsDigit)), out int value);
            return value;
        }

        public void ResetPlayerProgress()
        {
            _progressRepository.Delete();
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
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

        public void SaveProgress(PlayerProgressModel model)
        {
            _progressRepository.Set(model);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("levels.level")) {
                LevelDescriptor levelDescriptor = new LevelDescriptor();
                levelDescriptor.Configure(temp);
                _levelDescriptorRegistry.LevelDescriptors.Add(levelDescriptor);
            }
        }

        private void LoadRegionsDescriptors(Configuration config, object[] loadparameters)
        {
            foreach (Configuration configuration in config.GetList<Configuration>("regions.region")) {
                RegionDescriptor regionDescriptor = new RegionDescriptor();
                regionDescriptor.Configure(configuration);
                _regionDescriptorRegistry.RegionDescriptors.Add(regionDescriptor);
            }
        }

        private bool HasPlayerProgress()
        {
            return _progressRepository.Exists();
        }
    }
}