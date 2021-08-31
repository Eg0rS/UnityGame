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
using CircularScrollingList;
using DeliveryRush.Core.UI.Dialog;
using DeliveryRush.Inventory.Model;
using DeliveryRush.Inventory.Service;
using DeliveryRush.LevelMap.Levels.Descriptor;
using DeliveryRush.LevelMap.Levels.Service;
using DeliveryRush.Location.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace DeliveryRush.LevelMap.Levels.UI.LevelDiscription.DescriptionLevelDialog
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
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private GameService _gameService;

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
        private List<ViewDronPanel> _viewDronPanels = new List<ViewDronPanel>();
        private ListPositionCtrl _listPositionCtrl;
        private float _screenWidth;

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
            _screenWidth = Screen.width;
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
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items) {
                _uiService.Create<ViewDronPanel>(UiModel.Create<ViewDronPanel>(item).Container(itemContainer))
                          .Then(controller => { _viewDronPanels.Add(controller); })
                          .Done();
            }
            _uiService.Create<ScrollControllerForDescriptionDialog>(UiModel.Create<ScrollControllerForDescriptionDialog>(_viewDronPanels)
                                                                           .Container(itemContainer))
                      .Then(controller => {
                          _listPositionCtrl = controller._control;
                          if (_inventoryService.Inventory.Items.Count % 2 == 0) {
                              itemContainer.GetComponent<RectTransform>().localPosition = new Vector3(400, 0, 0);
                          }
                      })
                      .Done();
        }

        private void MoveLeft()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(1);
        }

        private void MoveRight()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(-1);
        }

        [UIOnClick("StartGameButton")]
        private void OnStartGameButton()
        {
            string dronId = "";
            foreach (ViewDronPanel panel in _viewDronPanels) {
                RectTransform transform = panel.gameObject.GetComponent<RectTransform>();
                if (transform.position.x >= _screenWidth / 3 && transform.position.x <= _screenWidth) {
                    dronId = panel.ItemId;
                }
            }
            _levelService.CurrentLevelId = _levelDescriptor.Id;
            _gameService.StartGame(_levelDescriptor, dronId);
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
            MoveLeft();
        }

        [UIOnClick("RightArrow")]
        private void OnRightClick()
        {
            MoveRight();
        }
    }
}