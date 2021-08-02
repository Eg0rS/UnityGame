using System;
using AgkUI.Binding.Attributes;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Descriptor.Service;
using DronDonDon.Game.Levels.Descriptor;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Resource.UI.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class DescriptionLevelDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfDescriptionLevelDialog@embeded";
        
        [Inject] 
        private IoCProvider<DialogManager> _dialogManager;
        
        [UIObjectBinding("Title")]
        private UILabel _title;
        
        [UIObjectBinding("Description")]
        private UILabel _description;
        
        [UIObjectBinding("ChipsTask")] 
        private UIButton _chipText;
        
        [UIObjectBinding("StrengthTask")] 
        private UIButton _strengthText;
        
        [UIObjectBinding("TimeTask")] 
        private UIButton _timeText;
        
        [UIObjectBinding("StartGameButton")] 
        private UIButton _startGameButton;

        private LevelDescriptor _levelDescriptor;

        public LevelDescriptor LevelDescriptor { get; set; }
        private void Start()
        {
            
        }

        private void DisplayTitle()
        {
            //_title.text = 
        }
    }
}