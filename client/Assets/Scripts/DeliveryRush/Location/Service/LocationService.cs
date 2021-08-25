using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Screens.Service;
using DeliveryRush.Core;
using DeliveryRush.Resource.Descriptor;
using DeliveryRush.Location.Service.Builder;
using DeliveryRush.Location.UI.Screen;
using DeliveryRush.Location.World.Dron.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

namespace DeliveryRush.Location.Service
{
    [Injectable]
    public class LocationService : GameEventDispatcher
    {
        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LocationBuilderManager _locationBuilderManager;

        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject]
        private IoCProvider<GameService> _gameService;

        [Inject]
        private DronService _dronService;

        public void SwitchLocation(LevelDescriptor levelDescriptor, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelDescriptor, dronId);
            Light lightOnLevel = GameObject.Find("DirectionalLight").GetComponent<Light>();
            lightOnLevel.color = Resources.Load<Material>(levelDescriptor.Color).color;
            RenderSettings.skybox = Resources.Load<Material>(levelDescriptor.Skybox);
            DynamicGI.UpdateEnvironment();
        }

        private void CreatedWorld(LevelDescriptor levelDescriptor, string dronId)
        {
            string levelPrefabName = levelDescriptor.Prefab;
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() => {
                                       SetDrone(dronId);
                                        _gameService.Require().StartGame(levelDescriptor, dronId);
                                   })
                                   .Done();
        }

        private void SetDrone(string dronId)
        {
            GameObject parent = GameObject.Find("DronCube");
            Instantiate(Resources.Load<GameObject>(_dronService.GetDronById(dronId).DronDescriptor.Prefab), parent.transform.position,
                        Quaternion.Euler(0, 0, 0), parent.transform);
        }
    }
}