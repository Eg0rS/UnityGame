using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Screens.Service;
using Drone.Core;
using Drone.Descriptor;
using Drone.Levels.Descriptor;
using Drone.Levels.Repository;
using Drone.Location.Service.Builder;
using Drone.Location.UI.Screen;
using Drone.Random.MersenneTwister;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Location.Service
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
        private DifficultDescriptors _difficultDescriptors;

        [Inject]
        private ProgressRepository _progressRepository;

        public void SwitchLocation(LevelDescriptor levelDescriptor)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedLevel(levelDescriptor);
        }

        private void CreatedLevel(LevelDescriptor levelDescriptor)
        {
            _locationBuilderManager.CreateDefault()
                                   .Difficult(_difficultDescriptors)
                                   .SetSeed(_progressRepository.Get().Seed)
                                   .LevelDescriptor(levelDescriptor)
                                   .GameWorldContainer()
                                   .Build();
        }
    }
}