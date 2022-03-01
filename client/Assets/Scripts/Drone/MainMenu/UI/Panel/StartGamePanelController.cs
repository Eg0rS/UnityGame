using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Element.Buttons;
using Drone.Inventory.Model;
using Drone.Inventory.Service;
using Drone.LevelMap.UI.DescriptionLevelDialog;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
using EndlessScroll;
using IoC.Attribute;
using RSG;
using UnityEngine;
using LocationService = Drone.Location.Service.LocationService;

namespace Drone.MainMenu.UI.Panel
{
    [UIController("UI_prototype/Panel/StartGamePanel/pfStartGamePanel@embeded")]
    public class StartGamePanelController : MonoBehaviour
    {
        [Inject]
        private UIService _uiService;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private LocationService _locationService;

        [UIComponentBinding("Scroll")]
        private EndlessScrollView _endlessScroll;
        [UIComponentBinding("StartButton")]
        private UIButton _startButton;

        private LevelDescriptor _currentLevelDescriptor;

        [UICreated]
        private void Init()
        {
            _currentLevelDescriptor = _levelService.GetCurrentLevelDescriptor();
            if (_currentLevelDescriptor == null) {
                _startButton.gameObject.SetActive(false);
            }
            _startButton.onClick.AddListener(OnStartGameButton);
            CreateDroneChoice();
        }

        private void CreateDroneChoice()
        {
            List<IPromise<ViewDronePanel>> promises = new List<IPromise<ViewDronePanel>>();
            foreach (InventoryItemModel item in _inventoryService.Inventory.Items) {
                IPromise<ViewDronePanel> promise = _uiService.Create<ViewDronePanel>(UiModel.Create<ViewDronePanel>(item).Container(_endlessScroll));
                promises.Add(promise);
            }
            Promise<ViewDronePanel>.All(promises).Done(resolved => _endlessScroll.Init());
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
            string droneId = _endlessScroll.MiddleElement.GetComponent<ViewDronePanel>().ItemId;
            _levelService.SelectedLevelId = _currentLevelDescriptor.Id;
            _levelService.SelectedDroneId = droneId;
            _locationService.SwitchLocation(_currentLevelDescriptor);
            Debug.Log($"Start level {_currentLevelDescriptor.Id}");
        }
    }
}