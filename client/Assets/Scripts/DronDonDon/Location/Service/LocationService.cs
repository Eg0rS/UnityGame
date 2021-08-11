using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Location.World.Dron.Service;
using DronDonDon.Settings.UI;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService: GameEventDispatcher
    {
        [Inject]
        private ScreenManager _screenManager;  
        
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject] 
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject] private UIService _uiService;

        [Inject] 
        private DronService _dronService;
        
        public void StartGame(string levelPrefabName, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName, dronId);
        }
        private void CreatedWorld(string levelPrefabName, string dronId)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() =>
                                   {
                                       SetDrone(dronId);
                                       _overlayManager.Require().CreateGameOverlay();
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }

        private void SetDrone(string dronId)
        {
            GameObject parent = GameObject.Find("DronCube");
            Instantiate(Resources.Load<GameObject>(_dronService.GetDronById(dronId).DronDescriptor.Prefab),
                parent.transform.position, Quaternion.Euler(0, 0, 0),
                parent.transform);
        }
    }
}