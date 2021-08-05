﻿using System;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using UnityEngine;
using UnityEngine.Rendering;

namespace DronDonDon.Game.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";
        private const string CHIPS_TASK = "Собрать {0} чипов";
        private const string DURABILITY_TASK = "Сохранить не менее {0}% груза";
        private const string TIME_TASK = "Уложиться в {0} мин.";
        
        private int _chipsGoal;
        private float _durabilityGoal;
        private int _timeGoal;
        
        private int _chipsLevelResult;
        private float _durabilityLevelResult;
        private int _timeLevelResult;
        
        private bool _chipsTaskCompleted = false;
        private bool _durabilityTaskCompleted = false;
        private bool _timeTaskCompleted = false;
        private int _tasksCompletedCount = 0;
        
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();

        private string _levelId; 
        
        [UIComponentBinding("ChipsStar")]
        private ToggleButton _chipsStar;
        
        [UIComponentBinding("DurabilityStar")]
        private ToggleButton _durabilityStar;
        
        [UIComponentBinding("TimeStar")]
        private ToggleButton _timeStar;
        
        [UIComponentBinding("ChipsTask")]
        private UILabel _chipsTaskLabel;
        
        [UIComponentBinding("DurabilityTask")]
        private UILabel _durabilityTaskLabel;
        
        [UIComponentBinding("TimeTask")]
        private UILabel _timeTaskLabel;
        
        [UICreated]
        public void Init(object[] arg)
        {
            _logger.Debug("[LevelFinishedDialog] Init()...");

            if (_chipsLevelResult > _chipsGoal)
            {
                _chipsTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (_durabilityLevelResult > _durabilityGoal)
            {
                _durabilityTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (_timeLevelResult < _timeGoal)
            {
                _timeTaskCompleted = true;
                _tasksCompletedCount++;
            }
            SetDialogStars();
            SetDialogLabels();
        }
        
        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _logger.Debug("[LevelFinishedDialog] RestartButtonClicked()...");
        }

        [UIOnClick("NextLevelButton")]
        private void NextLevelButtonClicked()
        {
            _logger.Debug("[LevelFinishedDialog] NextLevelButtonClicked()...");
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _logger.Debug("[LevelFinishedDialog] LevelMapButtonClicked()...");
        }

        private void SetDialogStars()
        {
            _logger.Debug("[LevelFinishedDialog] SetDialogStars()...");
            _chipsStar.Interactable = false;
            _durabilityStar.Interactable = false;
            _timeStar.Interactable = false;
            
            _chipsStar.IsOn = _chipsTaskCompleted;
            _durabilityStar.IsOn = _durabilityTaskCompleted;
            _timeStar.IsOn = _timeTaskCompleted;
        }

        private void SetDialogLabels()
        {
            _logger.Debug("[LevelFinishedDialog] SetDialogLabels()...");
            _chipsTaskLabel.text = String.Format(CHIPS_TASK,_chipsGoal);
            _durabilityTaskLabel.text = String.Format(DURABILITY_TASK,_durabilityGoal);
            _timeTaskLabel.text = String.Format(TIME_TASK,_timeGoal);
        }
    }
}