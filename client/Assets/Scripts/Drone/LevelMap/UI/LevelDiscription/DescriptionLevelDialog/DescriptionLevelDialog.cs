using System;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using Drone.Core.UI.Dialog;
using Drone.Inventory.Model;
using Drone.Inventory.Service;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using LocationService = Drone.Location.Service.LocationService;

namespace Drone.LevelMap.UI.LevelDiscription.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class DescriptionLevelDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfDescriptionLevelDialog@embeded";

        [Inject]
        private UIService _uiService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private InventoryService _inventoryService;

        // [Inject]
        // private GameService _gameService;

        [Inject]
        private LocationService _locationService;

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
        private LevelDescriptor _levelDescriptor;
        private EndlessScrollView _endlessScroll;

        [UICreated]
        public void Init(LevelDescriptor levelDescriptor)
        {
            string chipText = "Собрать {0} чипов";
            string durabilityText = "Сохранить не менее {0}% груза";
            string timeText = "Уложиться в {0} сек.";
            _levelDescriptor = levelDescriptor;
            DisplayTitle();
            DisplayDescription();
            DisplayImage();
            DisplayTasks(chipText, durabilityText, timeText);
            CreateChoiseDron();
        }

        private void OnGUI()
        {
            if (UnityEngine.Event.current.Equals(UnityEngine.Event.KeyboardEvent("escape"))) {
                CloseDialog();
            }
        }

        private void DisplayTitle()
        {
            _title.text = _levelDescriptor.Exposition.Title;
        }

        private void DisplayDescription()
        {
            _description.text = _levelDescriptor.Exposition.Description;
        }

        private void DisplayTasks(string chipText, string durabilityText, string timeText)
        {
           // _chipText.text = String.Format(chipText, _levelDescriptor.Goals.NecessaryCountChips);
           // _durabilityText.text = String.Format(durabilityText, _levelDescriptor.Goals.NecessaryDurability);
          //  _timeText.text = String.Format(timeText, _levelDescriptor.Goals.NecessaryTime);
        }

        private void DisplayImage()
        {
           // _cargoImage.GetComponent<Image>().sprite = Resources.Load(_levelDescriptor.GameData.Image, typeof(Sprite)) as Sprite;
        }

        private void CreateChoiseDron()
        {
            GameObject itemContainer = GameObject.Find("ScrollContainer");
            _endlessScroll = itemContainer.GetComponent<EndlessScrollView>();
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items) {
                _uiService.Create<ViewDronePanel>(UiModel.Create<ViewDronePanel>(item).Container(itemContainer))
                          .Then(controller => { _endlessScroll.ScrollPanelList.Add(controller.gameObject); })
                          .Then(() => { _endlessScroll.Init(); })
                          .Done();
            }
        }

        private void MoveLeft()
        {
            _endlessScroll.MoveRight();
        }

        private void MoveRight()
        {
            _endlessScroll.MoveLeft();
        }

        [UIOnClick("StartGameButton")]
        private void OnStartGameButton()
        {
            string dronId = _endlessScroll.MiddleElement.GetComponent<ViewDronePanel>().ItemId;
            _levelService.SelectedLevelId = _levelDescriptor.Id;
            _levelService.SelectedDroneId = dronId;
            _locationService.SwitchLocation(_levelDescriptor);
            //_gameService.StartGame(_levelDescriptor, dronId);
        }

        [UIOnClick("pfBackground")]
        private void CloseDialog()
        {
            _dialogManager.Hide(gameObject);
        }

        [UIOnClick("LeftArrow")]
        private void OnLeftClick()
        {
            MoveLeft();
        }

        [UIOnClick("RightArrow")]
        private void OnRightClick()
        {
            MoveRight();
        }
    }
}