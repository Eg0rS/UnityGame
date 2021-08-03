using System;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Descriptor.Service;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using UnityEngine.UIElements;
using DronDonDon.Location.Service;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using LocationService = DronDonDon.Location.Service.LocationService;

namespace DronDonDon.Resource.UI.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class DescriptionLevelDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfDescriptionLevelDialog@embeded";

        [Inject] 
        private LocationService _locationService;
        
        [Inject] 
        private LevelService _levelService;
        
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [UIComponentBinding("Title")]
        private UILabel _title;
        
        [UIComponentBinding("Description")]
        private UILabel _description;
        
        [UIComponentBinding("ChipsTask")] 
        private UILabel _chipText;
        
        [UIComponentBinding("StrengthTask")] 
        private UILabel _strengthText;
        
        [UIComponentBinding("TimeTask")] 
        private UILabel _timeText;

        [UIObjectBinding("CargoImage")] 
        private GameObject _cargoImage;

        private LevelDescriptor _levelDescriptor;
        
        private GameObject _fog;

        [UICreated]
        public void Init(LevelDescriptor levelDescriptor)
        {
            FindFog();
            _levelDescriptor = levelDescriptor;
            DisplayTitle();
            DisplayDescription();
            DisplayTasks();
            DisplayImage();
        }
        
        private void DisplayTitle()
        {
            _title.text = _levelDescriptor.LevelTitle;
        }

        private void DisplayDescription()
        {
            _description.text = _levelDescriptor.LevelDescription;
        }

        private void DisplayTasks()
        {
            _chipText.text += _levelDescriptor.NecessaryCountChips + " чипов";
            _strengthText.text += _levelDescriptor.NecessaryCountStrength + " процентов";
            _timeText.text += _levelDescriptor.NecessaryTime + " минуты";
        }

        [UIOnClick("StartGameButton")]
        private void OnStartGameButton()
        {
            _locationService.StartGame(_levelDescriptor.Prefab);
            _levelService.CurrentLevelId = _levelDescriptor.Id;
        }
        
        private void OnMouseDown()
        {
            _dialogManager.Require().Hide(gameObject);
        }

        private void DisplayImage()
        {
            _cargoImage.GetComponent<Image>().sprite = Resources.Load(_levelDescriptor.LevelImage, typeof(Sprite)) as Sprite;
        }

        private void FindFog()
        {
            _fog = GameObject.Find("pfShadowFog(Clone)");
            _fog.transform.SetParent(transform);
        }
        
        //[UIOnClick("pfShadowFog(Clone)")]
        private void CloseDialog()
        {
            _dialogManager.Require().Hide(gameObject);
        }
    }
}