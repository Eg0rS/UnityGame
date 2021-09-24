﻿using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using Drone.Core.Audio.Model;
using Drone.Core.Audio.Service;
using Drone.Core.UI.Dialog;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Service;
using Drone.LevelMap.Zones.Descriptor;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using static System.String;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";

        private const string TASKS_COMPLETED = "Выполнено заданий {0} из 3";
        private const string CHIPS_TASK_FULL = "Собрано чипов {0} из {1}";
        private const string DURABILITY_TASK = "Сохранить не менее {0}% груза";
        private const string TIME_TASK = "Уложиться в {0} сек.";

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
        private LevelViewModel _levelViewModel;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;

        [UIComponentBinding("ChipsStar")]
        private ToggleButton _chipsStar;

        [UIComponentBinding("NextLevelButton")]
        private UIButton _nextLevelButton;

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

        [Inject]
        private SoundService _soundService;

        [UICreated]
        public void Init()
        {
            _levelId = _levelService.CurrentLevelId;
            _logger.Debug("[LevelFinishedDialog] Init()...");
            _levelViewModel = _levelService.GetLevels().Find(it => it.LevelProgress.Id.Equals(_levelId));

            _chipsGoal = _levelViewModel.LevelDescriptor.NecessaryCountChips;
            _durabilityGoal = _levelViewModel.LevelDescriptor.NecessaryDurability;
            _timeGoal = _levelViewModel.LevelDescriptor.NecessaryTime;

            _chipsLevelResult = _levelViewModel.LevelProgress.CountChips;
            _durabilityLevelResult = _levelViewModel.LevelProgress.Durability;
            _timeLevelResult = (int) _levelViewModel.LevelProgress.TransitTime;

            if (_chipsLevelResult >= _chipsGoal) {
                _chipsTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (_durabilityLevelResult >= _durabilityGoal) {
                _durabilityTaskCompleted = true;
                _tasksCompletedCount++;
            }

            if (_timeLevelResult <= _timeGoal) {
                _timeTaskCompleted = true;
                _tasksCompletedCount++;
            }

            SetDialogStars();
            SetDialogLabels();
            SetButtonNextActivity();
        }

        private void PlaySound(Sound sound)
        {
            _soundService.StopAllSounds();
            _soundService.PlaySound(sound);
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
            _tasksCompletedLabel.text = Format(TASKS_COMPLETED, _tasksCompletedCount);
            _chipsTaskLabel.text = Format(CHIPS_TASK_FULL, _chipsLevelResult, _chipsGoal);
            _durabilityTaskLabel.text = Format(DURABILITY_TASK, _durabilityGoal);
            _timeTaskLabel.text = Format(TIME_TASK, _timeGoal);
        }

        private void SetButtonNextActivity()
        {
            ZoneDescriptor zoneDescriptor = _levelService.GetZonesDescriptors().Find(x => x.LevelId.Contains(_levelViewModel.LevelProgress.Id));
            ZoneDescriptor nextZoneDescriptor = _levelService.GetZoneDescriptorById(_levelService.GetNextZoneId(zoneDescriptor.Id));
            if (_levelService.GetNextLevel() == 0 && _levelService.GetNextLevelId(_levelViewModel.LevelDescriptor.Id) == null) {
                _nextLevelButton.gameObject.SetActive(false);
                return;
            }
            if (nextZoneDescriptor == null) {
                _nextLevelButton.gameObject.SetActive(true);
                return;
            }
            if (_levelViewModel.LevelDescriptor.Type == LevelType.BOSS) {
                _nextLevelButton.gameObject.SetActive(_levelService.CompletedZoneConditions(zoneDescriptor.Id, nextZoneDescriptor.CountStars));
                return;
            }
            _nextLevelButton.gameObject.SetActive(true);
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelId);
        }

        [UIOnClick("NextLevelButton")]
        private void NextLevelButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelService.GetNextLevelId(_levelViewModel.LevelDescriptor.Id));
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }
    }
}