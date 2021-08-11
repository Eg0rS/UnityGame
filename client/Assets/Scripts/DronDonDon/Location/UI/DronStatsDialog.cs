
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using DronDonDon.Game.LevelDialogs;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;


namespace DronDonDon.Location.UI
{
    [UIController("UI/Dialog/pfGameOverlay@embeded")]
    public class DronStatsDialog :MonoBehaviour
    {
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [UIOnClick("PauseButton")]
        private void OnPauseButton()
        {
            _dialogManager.Require().ShowModal<LevelPauseDialog>();
        }
        
        
        
    }
}