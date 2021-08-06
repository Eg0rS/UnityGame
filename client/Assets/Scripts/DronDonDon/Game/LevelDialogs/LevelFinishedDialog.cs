using System;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Game.Levels.Model;
using DronDonDon.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Game.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";
        
        private const string TASKS_COMPLETED = "Выполнено заданий {0} из 3";
        private const string STARS_COLLECTED = "Получено звёзд {0} из 3";
        private const string CHIPS_TASK = "Собрать {0} чипов";
        private const string CHIPS_TASK_FULL = "Собрано чипов {0} из {1}";
        private const string DURABILITY_TASK = "Сохранить не менее {0}% груза";
        private const string DURABILITY_TASK_FULL = "Груз сохранён на {0}%";
        private const string TIME_TASK = "Уложиться в {0} сек.";
        private const string TIME_TASK_FULL = "Уровень пройден за {0} сек.";
        
        private int _chipsGoal;
        private float _durabilityGoal;
        private int _timeGoal;
        
        private int _chipsLevelResult;
        private float _durabilityLevelResult;
        private int _timeLevelResult;
        
        private bool _chipsTaskCompleted;
        private bool _durabilityTaskCompleted;
        private bool _timeTaskCompleted;
        private int _tasksCompletedCount;
        
        private string _levelId;
        
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject]
        private ScreenManager _screenManager;
        
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
        
        [UIComponentBinding("TasksCompletedTitle")]
        private UILabel _tasksCompletedLabel;
        
        [UICreated]
        public void Init(LevelViewModel levelViewModel)
        {
            _logger.Debug("[LevelFinishedDialog] Init()...");
            _tasksCompletedCount = 0;

            _chipsTaskCompleted = false;
            _durabilityTaskCompleted = false;
            _timeTaskCompleted = false;
            
            _levelId = levelViewModel.LevelDescriptor.Id;
            
            /*_chipsGoal = levelViewModel.LevelDescriptor.NecessaryCountChips;
            _durabilityGoal = levelViewModel.LevelDescriptor.NecessaryDurability;
            _timeGoal = levelViewModel.LevelDescriptor.NecessaryTime;

            _chipsLevelResult = levelViewModel.LevelProgress.CountChips;
            _durabilityLevelResult = levelViewModel.LevelProgress.Durability;
            _timeLevelResult = (int) levelViewModel.LevelProgress.TransitTime;*/

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
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            // _dialogManager.Require().Show<DescriptionLevelDialog>();
        }

        [UIOnClick("NextLevelButton")]
        private void NextLevelButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            // Обновляем id "текущего уровня" (если это не последний уровень)
            // Вызываем диалог информации об обновлённом (следующем) уровне
            // Или же отказываемся от этой кнопки...
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void SetDialogStars()
        {
            _chipsStar.Interactable = false;
            _durabilityStar.Interactable = false;
            _timeStar.Interactable = false;
            
            _chipsStar.IsOn = _chipsTaskCompleted;
            _durabilityStar.IsOn = _durabilityTaskCompleted;
            _timeStar.IsOn = _timeTaskCompleted;
        }

        private void SetDialogLabels()
        {
            _tasksCompletedLabel.text = String.Format(TASKS_COMPLETED, _tasksCompletedCount);
            _chipsTaskLabel.text = String.Format(CHIPS_TASK,_chipsGoal);
            _durabilityTaskLabel.text = String.Format(DURABILITY_TASK,_durabilityGoal);
            _timeTaskLabel.text = String.Format(TIME_TASK,_timeGoal);
            
            /*_tasksCompletedLabel.text = String.Format(STARS_COLLECTED, _tasksCompletedCount);
            _chipsTaskLabel.text = String.Format(CHIPS_TASK_FULL, _chipsLevelResult, _chipsGoal);
            _timeTaskLabel.text = String.Format(TIME_TASK_FULL,_timeLevelResult);
            _durabilityTaskLabel.text = String.Format(DURABILITY_TASK_FULL,_durabilityLevelResult);*/
        }
    }
}