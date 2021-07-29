using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Resources;
using DronDonDon.Core.Filter;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Repository;
using IoC.Attribute;
using UnityEngine;

namespace DronDonDon.Game.Levels.Service
{
    public class LevelService : IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject] 
        private ProgressRepository _progressRepository;

        private List<LevelViewModel> _levelsViewModels  = new List<LevelViewModel>();

        private List<LevelDescriptor> _levelsDescriptors;
        
        public void Init()
        {
            _resourceService.LoadConfiguration("Configs/levels@embeded", OnConfigLoaded);
            InitProgress();
        }
        
        public void InitProgress()
        {
            if (!HasPlayerProgress())
            {
                PlayerProgressModel model = new PlayerProgressModel();
                _progressRepository.Set(model);
            }
        }
        
        public void SaveProgress( PlayerProgressModel model)
        {
            _progressRepository.Set(model);
        }
        
        public void SetLevelProgress(string levelId, int countStars, int countChips, int transitTime)
        {
            PlayerProgressModel model = RequireProgressModel();
            LevelProgress levelProgress = GetLevelProgressById(levelId);
            levelProgress.CountChips = countChips;
            levelProgress.CountStars = countStars;
            levelProgress.TransitTime = transitTime;
            SaveProgress(model);
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
            PlayerProgressModel playerProgressModel = RequireProgressModel();
            for (int i = 0; i < playerProgressModel.LevelsProgress.Count; i++)
            {
                LevelViewModel levelViewModel = new LevelViewModel();
                levelViewModel.LevelDescriptor = _levelsDescriptors[i];
                levelViewModel.LevelProgress = playerProgressModel.LevelsProgress[i];
                _levelsViewModels.Add(levelViewModel);
            } 
            return _levelsViewModels;
        }
        
        public LevelDescriptor GetLevelDescriptorByID(string id)
        {
            return _levelsDescriptors.Find(x => x.Id.Equals(id));
        }

        public List<LevelDescriptor> GetListLevelsDescriptors()
        {
            return _levelsDescriptors;
        }

        public LevelProgress GetLevelProgressById(string levelId)
        {
            PlayerProgressModel playerModel = RequireProgressModel();
            LevelProgress level = playerModel.LevelsProgress.Find(x => x.Id.Equals(levelId));
            return level;
        }
        
        public void DeletePlayerProgress()
        {
            _progressRepository.Delete();
            PlayerProgressModel model = new PlayerProgressModel();
            SaveProgress(model);
        }

        public void CreateLevelById(string id)
        {
            PlayerProgressModel model = RequireProgressModel();
            LevelProgress levelProgress = new LevelProgress();
            levelProgress.Id = id;
            model.LevelsProgress.Add(levelProgress);
            SaveProgress(model);
        }
        
        private bool HasPlayerProgress()
        {
            return (_progressRepository.Get() != null);
        }
        
        private PlayerProgressModel RequireProgressModel()
        {
            PlayerProgressModel model = _progressRepository.Get();
            return model;
        }
        
        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            _levelsDescriptors = new List<LevelDescriptor>();
            foreach (Configuration temp in config.GetList<Configuration>("levels.level"))
            {
                LevelDescriptor levelDescriptor = new LevelDescriptor();
                levelDescriptor.Configure(temp);
                _levelsDescriptors.Add(levelDescriptor);
            }
        }
    }
}