using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.Core.UI.Dialog;
using Drone.Inventory.Model;
using Drone.Inventory.Service;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using IoC.Attribute;
using UnityEngine;
using LocationService = Drone.Location.Service.LocationService;

namespace Drone.LevelMap.UI.LevelDiscription.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class DescriptionLevelDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/DescriptionLevel/pfDescriptionLevelDialog@embeded";

        [Inject]
        private UIService _uiService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private LocationService _locationService;

        [UIComponentBinding("LevelName")]
        private UILabel _title;

        [UIComponentBinding("Close")]
        private UIButton _closeButton;

        private LevelDescriptor _levelDescriptor;
        private EndlessScrollView _endlessScroll;

        [UICreated]
        public void Init(LevelDescriptor levelDescriptor)
        {
            string chipText = "Собрать {0} чипов";
            string durabilityText = "Сохранить не менее {0}% груза";
            string timeText = "Уложиться в {0} сек.";
            _levelDescriptor = levelDescriptor;
            _closeButton.onClick.AddListener(CloseDialog);
            DisplayTitle();
            
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

        private void CreateChoiseDron()
        {
            GameObject itemContainer = GameObject.Find("Scroll");
            _endlessScroll = itemContainer.GetComponent<EndlessScrollView>();
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items) {
                _uiService.Create<ViewDronePanel>(UiModel.Create<ViewDronePanel>(item).Container(itemContainer))
                          .Then(controller => { _endlessScroll.ScrollPanelList.Add(controller.gameObject); })
                          .Then(() => { _endlessScroll.Init(); })
                          .Done();
            }
        }
        [UIOnClick("Left")]
        private void MoveLeft()
        {
            _endlessScroll.MoveRight();
        }
        [UIOnClick("Right")]
        private void MoveRight()
        {
            _endlessScroll.MoveLeft();
        }

        [UIOnClick("StartButton")]
        private void OnStartGameButton()
        {
            string dronId = _endlessScroll.MiddleElement.GetComponent<ViewDronePanel>().ItemId;
            _levelService.SelectedLevelId = _levelDescriptor.Id;
            _levelService.SelectedDroneId = dronId;
            _locationService.SwitchLocation(_levelDescriptor);
        }
        
        private void CloseDialog()
        {
            _dialogManager.Hide(gameObject);
        }
        
    }
}