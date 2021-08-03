using System;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.UI;

namespace DronDonDon.Game.LevelDialogs.LevelFinished
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();

        private const string TASK_COUNT_TITLE = "Выполнено заданий {0} из 3";
        private const string CHIPS_TASK = "Собрано чипов — {0} из {1}";
        private const string STRENGTH_TASK = "Прочность груза — {0}%";
        private const string TIME_TASK = "Время прохождения — {0}";
        
        [Inject]
        private LevelService _levelService;

        [UIObjectBinding("RestartButton")]
        private UIButton _restartButton;
        
        [UIObjectBinding("NextLevelButton")]
        private UIButton _nextLevelButton;

        [UIObjectBinding("LevelMapButton")]
        private UIButton _levelMapButton;

        [UIObjectBinding("ChipsStar")]
        private GameObject _chipsStar;
        
        [UIObjectBinding("StrengthStar")]
        private GameObject _strengthStar;
        
        [UIObjectBinding("TimeStar")]
        private GameObject _timeStar;
        
        [UIObjectBinding("ChipsTask")]
        private GameObject _chipsTask;
        
        [UIObjectBinding("StrengthTask")]
        private GameObject _strengthTask;
        
        [UIObjectBinding("TimeTask")]
        private GameObject _timeTask;

        public void CalculateResult(string levelId, int chips, int strength, int time)
        {
            PlayerProgressModel model = _levelService.RequireProgressModel();
            
            // LevelDescriptor levelDescriptor = _levelService.GetLevelDescriptorByID(levelId);
            // PlayerProgressModel model = _levelService.RequireProgressModel();
            // string currentLevel = model.CurrentLevel;
            
            // Результат и количество выполненных заданий
            bool chipsTaskGoal = false;
            bool strengthTaskGoal = false;
            bool timeTaskGoal = false;
            int goalCount = 0;

            // А это как бы результат прохождения уровня
            int level_chips = 47;
            int level_strength = 36;
            int level_time = 153;

            // Если собрали достаточное количество чипов
            if (level_chips > chips)
            {
                chipsTaskGoal = true;
                goalCount += 1;
            }
            else
            {
                
            }

            // Если не ушатали дрон
            if (level_strength > strength)
            {
                strengthTaskGoal = true;
                goalCount += 1;
            }
            else
            {
                
            }

            // Если успели вовремя
            if (level_time < time)
            {
                timeTaskGoal = true;
                goalCount += 1;
            }
            else
            {
                
            }
            
            chipsTaskGoal = level_chips > chips;
            strengthTaskGoal = level_strength > strength;
            timeTaskGoal = level_time < time;

            goalCount = 2;
            int chips_res = chips;
            int strength_res = strength / level_strength;
            int time_res = 180;
            
            _logger.Debug("[LevelFinishedDialog] На уровне " + levelId +
                          " собрано чипов — " + level_chips +
                          ", прочность — " + level_strength +
                          ", время — " + level_time);
            
            _logger.Debug("[LevelFinishedDialog] Итоговые результаты: " +
                          String.Format(TASK_COUNT_TITLE, goalCount) + ", " +
                          String.Format(CHIPS_TASK, chips_res, level_chips) + ", " +
                          String.Format(STRENGTH_TASK, strength_res) + ", " +
                          String.Format(TIME_TASK, time_res));
        }

        private void _SwitchStar(GameObject gameObject, bool flag)
        {
            
        }
    }
}