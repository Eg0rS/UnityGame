using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Screens.Service;
using DeliveryRush.Core;
using DeliveryRush.LevelMap.Levels.Descriptor;
using DeliveryRush.Location.Service.Builder;
using DeliveryRush.Location.UI.Screen;
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

        public void SwitchLocation(LevelDescriptor levelDescriptor)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelDescriptor);
        }

        private void CreatedWorld(LevelDescriptor levelDescriptor)
        {
            string levelPrefabName = levelDescriptor.Prefab;
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() => Dispatch(new WorldEvent(WorldEvent.WORLD_CREATED)))
                                   .Done();
        }
    }
}