using System;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using UnityEngine;

namespace DronDonDon.Game.LevelDialogs.LevelFinished
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();

        private string _levelId;

        [UIObjectBinding("RestartButton")]
        private GameObject _restartButton;
        
        [UIObjectBinding("NextLevelButton")]
        private GameObject _nextLevelButton;

        [UIObjectBinding("LevelMapButton")]
        private GameObject _levelMapButton;

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
        // public void Init(string levelId, 
        //                  bool chipsTaskCompleted, 
        //                  bool durabilityTaskCompleted, 
        //                  bool timeTaskCompleted, 
        //                  string chipsTaskTitle, 
        //                  string durabilityTaskTitle, 
        //                  string timeTaskTitle)
        public void Init(object[] arg)
        {
            _logger.Debug("[LevelFinishedDialog] Итоговые результаты уровня " + levelId + ": " +
                          chipsTaskTitle + ", " +
                          durabilityTaskTitle + ", " +
                          timeTaskTitle);
            _levelId = levelId;
            
            _chipsStar.Interactable = false;
            _durabilityStar.Interactable = false;
            _timeStar.Interactable = false;
            
            _chipsStar.IsOn = chipsTaskCompleted;
            _durabilityStar.IsOn = durabilityTaskCompleted;
            _timeStar.IsOn = timeTaskCompleted;
            
            _chipsTaskLabel.GetComponent<UILabel>().text = chipsTaskTitle;
            _durabilityTaskLabel.GetComponent<UILabel>().text = durabilityTaskTitle;
            _timeTaskLabel.GetComponent<UILabel>().text = timeTaskTitle;
        }
    }
}