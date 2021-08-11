using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using DronDonDon.Core.Filter;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Event;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Repository;
using DronDonDon.Resource.UI.DescriptionLevelDialog;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Game.Levels.Service
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
        
        private List<LevelViewModel> _levelsViewModels  = new List<LevelViewModel>();
        public string CurrentLevelId { get; set; }

        public void Init()
        {
            if (_levelDescriptorRegistry.LevelDescriptors.Count == 0)
            {
                _resourceService.LoadConfiguration("Configs/levels@embeded", OnConfigLoaded);
            }
            if (!HasPlayerProgress())
            {
                InitProgress();
            }
        }
        
        public void InitProgress()
        {
            PlayerProgressModel model = new PlayerProgressModel
            {
                NextLevel = _levelDescriptorRegistry.LevelDescriptors[0].Id
            };
            /*LevelProgress levelProgress = new LevelProgress
            {
                Id = model.NextLevel, CountChips = 0, CountStars = 0, TransitTime = 0, Durability = 0
            };*/
            //model.LevelsProgress.Add(levelProgress);
            SaveProgress(model);
        }

        public void ShowStartLevelDialog(string levelId)
        {
            LevelDescriptor levelDescriptor = _levelsViewModels.Find(x => x.LevelDescriptor.Id.Equals(levelId)).LevelDescriptor;
            _dialogManager.Require().Show<DescriptionLevelDialog>(levelDescriptor);
        }

        public string GetCurrentLevelPrefab()
        {
            PlayerProgressModel model = RequireProgressModel();
            return  _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Id.Equals(model.NextLevel)).Prefab;
        }

        public void SetLevelProgress(string levelId, int countStars, int countChips, int transitTime,int durability,bool isCompleted, bool isCurrent)
        {
            PlayerProgressModel model = RequireProgressModel();
            LevelProgress levelProgress = GetLevelProgressById(levelId);
            levelProgress.CountChips = countChips;
            levelProgress.CountStars = countStars;
            levelProgress.TransitTime = transitTime;
            levelProgress.Durability = durability;
            levelProgress.IsCompleted = isCompleted;
            if (isCurrent && isCompleted)
            {
                LevelDescriptor descriptor = GetLevelDescriptorByID(levelId);
                LevelDescriptor nextDescriptor = GetLevelByOrder(descriptor.Order+1);
                if (nextDescriptor != null)
                {
                    model.NextLevel = nextDescriptor.Id;
                    CreateLevelById(nextDescriptor.Id);
                }
                else
                {
                    model.NextLevel = "EndGame";
                }
            }
            SaveProgress(model);
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
        }

        public LevelProgress GetLevelProgress(string levelId)
        {
            return GetLevelProgressById(levelId);
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
            PlayerProgressModel playerProgressModel = RequireProgressModel();
            foreach (LevelDescriptor item in _levelDescriptorRegistry.LevelDescriptors)
            {
                LevelViewModel levelViewModel = new LevelViewModel();
                levelViewModel.LevelDescriptor = item;
                levelViewModel.LevelProgress = playerProgressModel.LevelsProgress.Find(x => x.Id.Equals(item.Id));
                _levelsViewModels.Add(levelViewModel);
            }
            return _levelsViewModels;
        }

        public void ResetPlayerProgress()
        {
            InitProgress();
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
        }
        
        public LevelDescriptor GetLevelDescriptorByID(string id)
        {
            return _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Id.Equals(id));
        }
        
        public LevelProgress GetLevelProgressById(string levelId)
        {
            PlayerProgressModel playerModel = RequireProgressModel();
            LevelProgress level = playerModel.LevelsProgress.Find(x => x.Id.Equals(levelId));
            return level;
        }
        
        public PlayerProgressModel RequireProgressModel()
        {
            PlayerProgressModel model = _progressRepository.Require();
            return model;
        }
        
        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("levels.level"))
            {
                LevelDescriptor levelDescriptor = new LevelDescriptor();
                levelDescriptor.Configure(temp);
                _levelDescriptorRegistry.LevelDescriptors.Add(levelDescriptor);
            }
        }
        
        private void CreateLevelById(string id)
        {
            PlayerProgressModel model = RequireProgressModel();
            LevelProgress levelProgress = new LevelProgress();
            levelProgress.Id = id;
            model.LevelsProgress.Add(levelProgress);
            SaveProgress(model);
        }
        
        private LevelDescriptor GetLevelByOrder(int order)
        {
            try
            {
                return _levelDescriptorRegistry.LevelDescriptors.Find(x => x.Order.Equals(order));
            }
            catch(Exception e)
            {
                return null;
            }
        }
        
        private void SaveProgress( PlayerProgressModel model)
        {
            _progressRepository.Set(model);
        }
        
        private bool HasPlayerProgress()
        {
            return _progressRepository.Exists();
        }
    }
}