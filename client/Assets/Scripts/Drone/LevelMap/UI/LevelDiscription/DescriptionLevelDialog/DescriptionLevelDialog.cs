using System.Collections.Generic;
using AgkCommons.Extension;
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
using EndlessScroll;
using IoC.Attribute;
using RSG;
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
        [UIComponentBinding("StartButton")]
        private UIButton _startButton;

        [UIComponentBinding("Scroll")]
        private EndlessScrollView _endlessScroll;

        private LevelDescriptor _levelDescriptor;

        [UICreated]
        public void Init(LevelDescriptor levelDescriptor)
        {
            _levelDescriptor = levelDescriptor;
            _closeButton.onClick.AddListener(CloseDialog);
            _startButton.onClick.AddListener(OnStartGameButton);
            _startButton.Select();
            DisplayTitle();

            CreateChoiseDron().Then(() => _endlessScroll.Init());
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

        private IPromise CreateChoiseDron()
        {
            List<IPromise> promises = new List<IPromise>();
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items) {
                Promise promise = new Promise();
                promises.Add(promise);
                _uiService.Create<ViewDronePanel>(UiModel.Create<ViewDronePanel>(item).Container(_endlessScroll)).Then((e) => promise.Resolve());
            }
            return Promise.All(promises);
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

        private void OnStartGameButton()
        {
            string dronId = _endlessScroll.MiddleElement.GetComponent<ViewDronePanel>().ItemId;
            _levelService.SelectedLevelId = _levelDescriptor.Id;
            _levelService.SelectedDroneId = dronId;
            _locationService.SwitchLocation(_levelDescriptor);
        }

        [UIOnSwipe("ChoiseDrone")]
        private void OnSwipe()
        {
            Debug.Log(1);
        }

        private void CloseDialog()
        {
            _dialogManager.Hide(gameObject);
        }
    }
}