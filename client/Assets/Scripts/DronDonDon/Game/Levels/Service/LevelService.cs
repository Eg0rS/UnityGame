using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using DronDonDon.Core.Configurations;
using DronDonDon.Core.Filter;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using IoC.Attribute;
using NUnit.Framework;
using UnityEngine;

namespace DronDonDon.Game.Levels.Service
{
    public class LevelService : IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelService>();
        
        [Inject]
        private ResourceService _resourceService;

        [Inject] 
        private ProgressRepository _progressRepository;

        private List<LevelViewModel> _levelsViewModels;

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
        
        public void SaveProgress()
        {
            PlayerProgressModel model = RequireProgressModel();
            _progressRepository.Set(model);
        }
        
        public void SetChipsCount(string levelId,int value)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            level.CountChips = value;
        }

        public void SetStarsCount(string levelId,int value)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            level.CountStars = value;
        }

        public void SetTransitTime(string levelId,int value)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            level.TransitTime = value;
        }

        public int GetChipsCount(string levelId)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            return level.CountChips;
        }
        
        public int GetStarsCount(string levelId)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            return level.CountStars;
        }
        
        public float GetTransitTime(string levelId)
        {
            LevelProgress level = GetLevelProgressById(levelId);
            return level.TransitTime;
        }
        
        public bool HasPlayerProgress()
        {
            return (_progressRepository.Get() != null);
        }
        
        public PlayerProgressModel RequireProgressModel()
        {
            PlayerProgressModel model = _progressRepository.Get();
            return _progressRepository.Require();
        }
        
        public List<LevelViewModel> GetLevels()
        {
            if (_levelsViewModels == null)
            {
                PlayerProgressModel playerProgressModel = RequireProgressModel();
                for (int i = 0; i < _levelsDescriptors.Count-1; i++)
                {
                    LevelViewModel levelViewModel = new LevelViewModel();
                    levelViewModel.LevelDescriptor = _levelsDescriptors[i];
                    levelViewModel.LevelProgress = playerProgressModel.LevelsProgress[i];
                    _levelsViewModels.Add(levelViewModel);
                }
            }
            return _levelsViewModels;
        }
        
        public LevelDescriptor GetLevelDescriptorByID(string id)
        {
            return _levelsDescriptors.First(x => x.Id.Equals(id));
        }

        public List<LevelDescriptor> GetListLevelsDescriptors()
        {
            return _levelsDescriptors;
        }

        public LevelProgress GetLevelProgressById(string levelId)
        {
            PlayerProgressModel playerModel = RequireProgressModel();
            LevelProgress level = playerModel.LevelsProgress.First(x => x.Id.Equals(levelId));
            return level;
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