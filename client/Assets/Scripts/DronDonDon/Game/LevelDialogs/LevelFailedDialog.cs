using System;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using UnityEngine;

namespace DronDonDon.Game.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFailedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedDialog@embeded";
        
        private const string CHIPS_TASK = "Собрать {0} чипов";
        private const string DURABILITY_TASK = "Сохранить не менее {0}% груза";
        private const string TIME_TASK = "Уложиться в {0} мин.";
        
        private int _chipsGoal;
        private float _durabilityGoal;
        private int _timeGoal;
        private string _failReason = "";

        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();
        
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
        
        [UIComponentBinding("FailReason")]
        private UILabel _failReasonLabel;

        [UICreated]
        public void Init(object[] args)
        {
            _logger.Debug("[LevelFailedDialog] Init() ...");
            
            _chipsStar.Interactable = false;
            _durabilityStar.Interactable = false;
            _timeStar.Interactable = false;
            
            _chipsStar.IsOn = false;
            _durabilityStar.IsOn = false;
            _timeStar.IsOn = false;
            
            // TODO: определить, по какой причине игрок проиграл —
            // закончилась энергия или прочность
            
            _failReasonLabel.text = _failReason;
            _chipsTaskLabel.text = String.Format(CHIPS_TASK,_chipsGoal);
            _durabilityTaskLabel.text = String.Format(DURABILITY_TASK,_durabilityGoal);
            _timeTaskLabel.text = String.Format(TIME_TASK,_timeGoal);
        }
        
        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            
        }
    }
}