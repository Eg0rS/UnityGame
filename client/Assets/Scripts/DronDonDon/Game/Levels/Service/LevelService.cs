﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Resources;
using DronDonDon.Core.Filter;
using DronDonDon.Game.Levels.Event;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Repository;
using IoC.Attribute;
using UnityEngine;

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

        private List<LevelViewModel> _levelsViewModels  = new List<LevelViewModel>();
        
        
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
                CurrentLevel = _levelDescriptorRegistry.LevelDescriptors[0].Id
            };
            LevelProgress levelProgress = new LevelProgress
            {
                Id = model.CurrentLevel, CountChips = 0, CountStars = 0, TransitTime = 0
            };
            model.LevelsProgress.Add(levelProgress);
            SaveProgress(model);
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
        
        public void ResetPlayerProgress()
        {
            InitProgress();
            Dispatch(new LevelEvent(LevelEvent.UPDATED));
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
            return _progressRepository.Exists();
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
    }
}