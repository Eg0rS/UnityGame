using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Screens.Service;
using DeliveryRush.Core;
using DeliveryRush.Resource.Descriptor;
using DeliveryRush.Location.Service.Builder;
using DeliveryRush.Location.UI.Screen;
using DeliveryRush.World;
using DeliveryRush.World.Event;
using IoC.Attribute;
using IoC.Util;

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
        private GameWorld _gameWorld;

        public void SwitchLocation(LevelDescriptor levelDescriptor, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelDescriptor, dronId);
        }

        private void CreatedWorld(LevelDescriptor levelDescriptor, string dronId)
        {
            string levelPrefabName = levelDescriptor.Prefab;
            _locationBuilderManager.CreateDefault().Prefab(levelPrefabName).Build().Then(() => 
            {
                //_gameWorld.Dispatch(new WorldEvent(WorldEvent.WORLD_CREATED, levelDescriptor, dronId));
                _gameService.Require().StartGame(levelDescriptor,dronId);
            }).Done();
        }
    }
}