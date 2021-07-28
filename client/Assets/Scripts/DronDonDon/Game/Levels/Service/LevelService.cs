using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using DronDonDon.Core.Configurations;
using DronDonDon.Core.Filter;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.MainMenu.UI.Settings.IoC;
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

        private List<LevelViewModel> _levelsViewModels  = new List<LevelViewModel>();

        private List<LevelDescriptor> _levelDescriptors;
        
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
        
        public void SaveProgress(PlayerProgressModel model)
        {
            _progressRepository.Set(model);
        }

        public void SetLevelProgress(int countStars, int transitTime, int countChips,string id)
        {
            PlayerProgressModel model = RequireProgressModel();
            LevelProgress levelProgress = new LevelProgress();
            levelProgress.Id = id;
            levelProgress.CountChips = countChips;
            levelProgress.CountStars = countStars;
            levelProgress.TransitTime = transitTime;
            model.LevelsProgress.Add(levelProgress);
            SaveProgress(model);
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
            return model;
        }
        
        public List<LevelViewModel> GetLevels()
        {
            PlayerProgressModel model = RequireProgressModel();
            if (_levelsViewModels.Count == 0)
            {
                LevelProgress levelProgress = GetLevelProgressById("level1");
                
            }
            return _levelsViewModels;
        }
        
        public LevelDescriptor GetLevelDescriptorByID(string id)
        {
            return _levelDescriptors.First(x => x.Id.Equals(id));
        }

        public List<LevelDescriptor> GetListLevelsDescriptors()
        {
            return _levelDescriptors;
        }

        public LevelProgress GetLevelProgressById(string levelId)
        {
            PlayerProgressModel playerModel = RequireProgressModel();
            LevelProgress level = playerModel.LevelsProgress.Find(x => x.Id.Equals(levelId));
            if (level == null)
            {
                level = CreateLevelProgress(levelId);
                playerModel.LevelsProgress.Add(level);
                SaveProgress(playerModel);
            }
            return level;
        }

        private LevelProgress CreateLevelProgress(string levelId)
        {
            LevelProgress levelProgress = new LevelProgress();
            levelProgress.Id = levelId;
            return levelProgress;
        }
        
        public void DeletePlayerProgress()
        {
            _progressRepository.Delete();
        }

        public string GetAvaliebleLevelId()
        {
            PlayerProgressModel model = RequireProgressModel();
            if (model.LevelsProgress == null)
            {
                return "leve1";
            }
            if (_levelDescriptors.Count > model.LevelsProgress.Count)
            {
                return  $"level{model.LevelsProgress.Count}";
            }
            return $"level{_levelDescriptors.Count}";
        }
        
        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            _levelDescriptors = new List<LevelDescriptor>();
            foreach (Configuration temp in config.GetList<Configuration>("levels.level"))
            {
                LevelDescriptor levelDescriptor = new LevelDescriptor();
                levelDescriptor.Configure(temp);
                _levelDescriptors.Add(levelDescriptor);
            }
        }
    }
}