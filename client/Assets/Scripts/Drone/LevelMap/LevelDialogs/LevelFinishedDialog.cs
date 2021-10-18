using System;
using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.LevelMap.Levels.Model;
using Drone.LevelMap.Levels.Service;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.UI;
using static System.String;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";

        private const string CHIPS_TASK_GOAL = "Собрать {0} чипов";
        private const string CHIPS_TASK_SCORE = "Собрано {0} чипов";
        private const string DURABILITY_TASK_GOAL = "Сохранить не менее {0}% груза";
        private const string DURABILITY_TASK_SCORE = "Сохранено {0}% груза";
        private const string TIME_TASK_GOAL = "Пролететь за {0} сек.";
        private const string TIME_TASK_SCORE = "Пролет за {0} сек.";

        private string _levelId;

        private LevelViewModel _levelViewModel;
        private int _countStars;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;

        [UIObjectBinding("ChipsMark")]
        private GameObject _chipMark;
        [UIObjectBinding("DurabilityMark")]
        private GameObject _durabilityMark;
        [UIObjectBinding("TimerMark")]
        private GameObject _timerMark;
        [UIObjectBinding("StatisticsChipsTask")]
        private GameObject _chipTask;
        [UIObjectBinding("StatisticsDurabilityTask")]
        private GameObject _durabilityTask;
        [UIObjectBinding("StatisticsTimerTask")]
        private GameObject _timerTask;
        [UIObjectBinding("Stars")]
        private GameObject _starsContainer;

        [UIComponentBinding("LevelTitle")]
        private Text _levelTitle;

        [UICreated]
        public void Init()
        {
            _levelId = _levelService.SelectedLevelId;
            _levelViewModel = _levelService.GetLevels().Find(x => x.LevelDescriptor.Id.Equals(_levelId));
            _levelTitle.text = _levelViewModel.LevelDescriptor.Title;
            _countStars = 0;
            SetChipstask();
            SetDurabilityTask();
            SetTimeTask();
            SetStars();
        }

        private void SetStars()
        {
            List<GameObject> stars = _starsContainer.GetChildren();
            for (int i = 0; i < _countStars; i++) {
                stars[i].SetActive(true);
            }
        }

        private void SetTimeTask()
        {
            float goal = _levelViewModel.LevelDescriptor.NecessaryTime;
            float score = _levelViewModel.LevelProgress.TransitTime;
            SetTask(_timerTask, TIME_TASK_GOAL, TIME_TASK_SCORE, goal, score);
            if (score <= goal) {
                _timerMark.SetActive(true);
                _countStars++;
            }
        }

        private void SetDurabilityTask()
        {
            float goal = _levelViewModel.LevelDescriptor.NecessaryDurability;
            float score = _levelViewModel.LevelProgress.Durability;
            SetTask(_durabilityTask, DURABILITY_TASK_GOAL, DURABILITY_TASK_SCORE, goal, score);
            if (score >= goal) {
                _durabilityMark.SetActive(true);
                _countStars++;
            }
        }

        private void SetChipstask()
        {
            float goal = _levelViewModel.LevelDescriptor.NecessaryCountChips;
            float score = _levelViewModel.LevelProgress.CountChips;
            SetTask(_chipTask, CHIPS_TASK_GOAL, CHIPS_TASK_SCORE, goal, score);
            if (score >= goal) {
                _chipMark.SetActive(true);
                _countStars++;
            }
        }

        private void SetTask(GameObject container, string patternGoal, string patternValue, float goal, float score)
        {
            List<GameObject> statistics = container.GetChildren();
            foreach (GameObject obj in statistics) {
                if (obj.name.Equals("TaskGoal")) {
                    List<GameObject> goalContainers = obj.GetChildren();
                    foreach (GameObject goalObj in goalContainers) {
                        if (goalObj.name.Equals("GoalValue")) {
                            goalObj.GetComponent<Text>().text = Format(patternGoal, goal);
                        }
                    }
                } else if (obj.name.Equals("TaskScore")) {
                    List<GameObject> scoreContainers = obj.GetChildren();
                    foreach (GameObject scoreobj in scoreContainers) {
                        if (scoreobj.name.Equals("ScoreValue")) {
                            scoreobj.GetComponent<Text>().text = Format(patternValue, Math.Round(score, 2));
                        }
                    }
                }
            }
        }

        [UIOnClick("ButtonRetry")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelId);
        }

        [UIOnClick("ButtonNext")]
        private void NextLevelButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelService.GetNextLevelId(_levelViewModel.LevelDescriptor.Id));
        }

        [UIOnClick("ButtonMenu")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }
    }
}