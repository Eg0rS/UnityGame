using System;
using System.Collections.Generic;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Inventory.Model;
using DronDonDon.Inventory.Service;
using DronDonDon.Shop.Descriptor;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
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
        private UIService _uiService;
        
        [Inject] 
        private LevelService _levelService;
        
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject] 
        private ShopDescriptor _shopDescriptor;

        [Inject] 
        private InventoryService _inventoryService;
        
        [UIComponentBinding("Title")]
        private UILabel _title;
        
        [UIComponentBinding("Description")]
        private UILabel _description;
        
        [UIComponentBinding("ChipsTask")] 
        private UILabel _chipText;
        
        [UIComponentBinding("DurabilityTask")] 
        private UILabel _durabilityText;
        
        [UIComponentBinding("TimeTask")] 
        private UILabel _timeText;

        [UIObjectBinding("CargoImage")] 
        private GameObject _cargoImage;

        [UIObjectBinding("ScrollContainer")] 
        private GameObject _scrollContainer;
        
        private LevelDescriptor _levelDescriptor;

        private List<ViewDronPanel> _viewDronPanels = new List<ViewDronPanel>();
        
        private ListPositionCtrl _listPositionCtrl;

        private string _currentDronId;

        public string CurrentDronId { get; set; }
        
        
        [UICreated]
        public void Init(LevelDescriptor levelDescriptor)
        {
            string chipText = "Собрать {0} чипов";
            string durabilityText = "Сохранить не менее {0}% груза";
            string timeText = "Уложиться в {0} мин.";
            _levelDescriptor = levelDescriptor;
            DisplayTitle();
            DisplayDescription();
            DisplayTasks(chipText,durabilityText,timeText);
            DisplayImage();
            CreateChoiseDron();
        }
        
        private void DisplayTitle()
        {
            _title.text = _levelDescriptor.LevelTitle;
        }

        private void DisplayDescription()
        {
            _description.text = _levelDescriptor.LevelDescription;
        }

        private void DisplayTasks(string chipText, string durabilityText, string timeText)
        {
            _chipText.text = String.Format(chipText, _levelDescriptor.NecessaryCountChips);
            _durabilityText.text = String.Format(durabilityText, _levelDescriptor.NecessaryDurability);
            _timeText.text = String.Format(timeText, _levelDescriptor.NecessaryTime);
        }
        
        private void DisplayImage()
        {
            _cargoImage.GetComponent<Image>().sprite = Resources.Load(_levelDescriptor.LevelImage, typeof(Sprite)) as Sprite;
        }

        private void CreateChoiseDron()
        {
            GameObject itemContainer = GameObject.Find("ScrollContainer");
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items)
            {
                _uiService.Create<ViewDronPanel>(UiModel
                        .Create<ViewDronPanel>(item)
                        .Container(itemContainer))
                    .Then(controller =>
                    {
                        _viewDronPanels.Add(controller);
                    })
                    .Done();
            }
            if(_viewDronPanels.Count % 2 == 0){
                GameObject.Find("ScrollContainer").transform.Translate(447,0,0);
            }
            else
            {
                GameObject.Find("ScrollContainer").transform.Translate(0,0,0);
            }
            _uiService.Create<ScrollControllerForDescriptionDialog>(UiModel
                    .Create<ScrollControllerForDescriptionDialog>(_viewDronPanels)
                    .Container(itemContainer)).Then(
                    controller =>
                    {
                        _listPositionCtrl = controller.Control;
                        
                    })
                .Done();
        }
        
        private void MoveLeft()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(1);
        }
        private void MoveRight()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(-1);;
        }

        [UIOnClick("StartGameButton")]
        private void OnStartGameButton()
        {
            _locationService.StartGame(_levelDescriptor.Prefab);
            _levelService.CurrentLevelId = _levelDescriptor.Id;
        }
        
        [UIOnClick("pfBackground")]
        private void CloseDialog()
        {
            _dialogManager.Require().Hide(gameObject);
        }

        [UIOnSwipe("ScrollContainer")]
        private void OnSwipe(Swipe swipe)
        {
            if (swipe.Check(HorizontalSwipeDirection.LEFT)) {
                MoveLeft();
            } else if (swipe.Check(HorizontalSwipeDirection.RIGHT)) {
                MoveRight();
            }
        }
        
        [UIOnClick("LeftArrow")]
        private void OnLeftClick()
        {
            Debug.Log("click");
            MoveLeft();
        }
        [UIOnClick("RightArrow")]
        private void OnRightClick()
        {
            MoveRight();
        }
    }
}