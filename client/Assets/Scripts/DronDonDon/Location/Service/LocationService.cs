using System;
using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs.LevelFinished;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Location.World.Dron.Descriptor;
using DronDonDon.Location.World.Dron.IoC;
using DronDonDon.Location.World.Dron.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService 
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationService>();

        private string _levelId;
        private int _chipsGoal;
        private float _durabilityGoal;
        private int _timeGoal;

        private string _dronId;
        private float _dronInitialDurability;
        private float _durabilityPercent;

        private int _chipsLevelResult;
        private float _durabilityLevelResult;
        private int _timeLevelResult;
        
        private bool _chipsTaskCompleted;
        private bool _durabilityTaskCompleted;
        private bool _timeTaskCompleted;
        private int _tasksCompletedCount;
        
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        [Inject]
        private LevelService _levelService;
        [Inject]
        private DronService _dronService;

        public void StartGame(string levelPrefabName)
        {
            bool aasdsd = true;
            string a = "asd";
            /*_overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName);*/
            object[] asd = {aasdsd, a};
            _dialogManager.Require().Show<LevelFinishedDialog>(asd);
            
        }
        
        private void CreatedWorld(string levelPrefabName)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }

        public LocationService()
        {
            _chipsTaskCompleted = false;
            _durabilityTaskCompleted = false;
            _timeTaskCompleted = false;
            _tasksCompletedCount = 0;
        }

        private void OnLevelFinish()
        {
            string CHIPS_TASK = "Необходимо собрать {0} чипов";
            string DURABILITY_TASK = "Необходимая прочность более {0}%";
            string TIME_TASK = "Время прохождения менее {0} мин.";
            
            GetLevelInfo();
            GetDronInfo();
            CalculateResult(_levelId, _chipsLevelResult, _durabilityLevelResult, _timeLevelResult);
        }

        private void CalculateResult(string levelId, int chipsRes, float durabilityRes, int timeRes)
        {
            // Процент повреждений относительно ИСХОДНЫХ значений самого дрона
            _durabilityPercent = Mathf.Round(durabilityRes / _dronInitialDurability * 100);

            if (chipsRes >= _chipsGoal)
            {
                _chipsTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (_durabilityPercent >= _durabilityGoal)
            {
                _durabilityTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (timeRes <= _timeGoal)
            {
                _timeTaskCompleted = true;
                _tasksCompletedCount++;
            }
            
            // _logger.Debug("[LevelFinishedDialog] На уровне " + levelId +
            //               " собрано чипов — " + chipsRes + " из " + _chipsGoal +
            //               ", прочность — " + _durabilityPercent + " (минимум " + _durabilityGoal + ")" +
            //               ", время прохождения — " + timeRes + " из " + _timeGoal + " сек.");
            //
            // _logger.Debug("[LevelFinishedDialog] Итоговые результаты: " +
            //               String.Format(TASK_COUNT_TITLE, _tasksCompletedCount) + ", " +
            //               String.Format(CHIPS_TASK, chipsRes, _chipsGoal) + ", " +
            //               String.Format(DURABILITY_TASK, _durabilityPercent) + ", " +
            //               String.Format(TIME_TASK, TimeSpan.FromSeconds(timeRes).Minutes+":"+TimeSpan.FromSeconds(timeRes).Seconds));
        }

        private void GetLevelInfo()
        {
            /*
             * Как бы сыграли, сохраняем прогресс... 
             */
            _levelService.SetLevelProgress("level1", 2, 53, 194,true, true);
            
            LevelProgress levelProgress = _levelService.GetLevelProgressById(_levelId);
            LevelDescriptor levelDescriptor = _levelService.GetLevelDescriptorByID(_levelId);
            
            // Целевые значения для примера
            _levelId = "level1";     // TODO: брать из levelService.CurrentLevelId ???
            _chipsGoal = 50;         // levelDescriptor.GetChips
            _durabilityGoal = 80;    // levelDescriptor.GetDurability    
            _timeGoal = 179;         // levelDescriptor.GetTime

            _chipsLevelResult = levelProgress.CountChips;
            _durabilityLevelResult = 90;                        // levelProgress.Durability;
            _timeLevelResult = (int) levelProgress.TransitTime;
        }

        private void GetDronInfo()
        {
            _dronInitialDurability = _dronService.GetDronById(_dronId).DronDescriptor.Durability;
        }
        
    }
}