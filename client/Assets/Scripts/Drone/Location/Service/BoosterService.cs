using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Location.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;

namespace Drone.Location.Service
{
    public class BoosterService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private ResourceService _resourceService;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public bool IsShieldActivate { get; private set; }

        private Dictionary<string, BoosterDescriptor> _boosterDescriptors;

        public void Init()
        {
            _boosterDescriptors = new Dictionary<string, BoosterDescriptor>();
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SPEED, OnTakeSpeed);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SHIELD, OnTakeShield);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("boosters.booster")) {
                BoosterDescriptor boosterDescriptor = new BoosterDescriptor();
                boosterDescriptor.Configure(conf);
                _boosterDescriptors.Add(boosterDescriptor.Type, boosterDescriptor);
            }
        }

        [NotNull]
        public BoosterDescriptor GetDescriptorByType(WorldObjectType objectType)
        {
            string type = Enum.GetName(typeof(WorldObjectType), objectType);
            return _boosterDescriptors[type];
        }

        private void OnTakeSpeed(WorldEvent worldEvent)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SPEED, GetDescriptorByType(WorldObjectType.SPEED_BOOSTER)));
            Invoke(nameof(DisableSpeed), float.Parse(GetDescriptorByType(WorldObjectType.SPEED_BOOSTER).Params["Duration"]));
        }

        private void DisableSpeed()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SPEED, GetDescriptorByType(WorldObjectType.SPEED_BOOSTER)));
        }

        private void OnTakeShield(WorldEvent worldEvent)
        {
            IsShieldActivate = true;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SHIELD));
            Invoke(nameof(DisableShield), float.Parse(GetDescriptorByType(WorldObjectType.SHIELD_BOOSTER).Params["Duration"]));
        }

        private void DisableShield()
        {
            IsShieldActivate = false;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SHIELD));
        }
    }
}