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
    public class LevelFailedCompactDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedCompactDialog@embeded";

        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFinishedDialog>();
        private string _failReason = "";
        
        [UIComponentBinding("FailReasonTitle")]
        private UILabel _failReasonLabel;

        [UICreated]
        public void Init(object[] args)
        {
            _logger.Debug("[LevelFailedCompactDialog] Init()...");
            
            // TODO: определить, по какой причине игрок проиграл —
            // закончилась энергия или прочность
            
            SetDialogLabels();
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _logger.Debug("[LevelFailedCompactDialog] RestartButtonClicked()...");

        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _logger.Debug("[LevelFailedCompactDialog] LevelMapButtonClicked()...");

        }

        private void SetDialogLabels()
        {
            _logger.Debug("[LevelFailedCompactDialog] SetDialogLabels()...");
            _failReasonLabel.text = _failReason;
        }
    }
}