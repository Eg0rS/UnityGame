using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using Drone.Booster.Descriptor;
using Drone.Core.Filter;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Service;
using Drone.Location.World.Drone;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Booster.Service
{
    public class BoosterService : GameEventDispatcher, IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private GameService _gameService;

        [Inject]
        private DroneAnimService _droneAnimService;

        private List<BoosterDescriptor> _boosterDescriptors;
        private DroneModel _droneModel;

        private BoosterDescriptor _speedBoosterDescriptor;
        private BoosterDescriptor _shieldBoosterDescriptor;
        public void Init()
        {
            _boosterDescriptors = new List<BoosterDescriptor>();
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
            _gameService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("boosters.booster")) {
                BoosterDescriptor boosterDescriptor = new BoosterDescriptor();
                boosterDescriptor.Configure(conf);
                _boosterDescriptors.Add(boosterDescriptor);
            }
        }

        [NotNull]
        private BoosterDescriptor GetDescriptorByType(WorldObjectType objectType)
        {
            string type = Enum.GetName(typeof(WorldObjectType), objectType);

            foreach (BoosterDescriptor boosterDescriptor in _boosterDescriptors) {
                if (boosterDescriptor.Type == type) {
                    return boosterDescriptor;
                }
            }
            throw new Exception("BoosterDescriptor not found");
        }

        private void OnWorldCreated(WorldEvent objectEvent)
        {
            _droneModel = objectEvent.DroneModel;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
        }

        private void OnDronCollision(WorldEvent worldEvent)
        {
            WorldObjectType objectType = worldEvent.CollisionObject.gameObject.GetComponent<PrefabModel>().ObjectType;
            switch (objectType) {
                case WorldObjectType.SPEED_BOOSTER:
                    OnTakeSpeed(GetDescriptorByType(objectType));
                    break;
                case WorldObjectType.SHIELD_BOOSTER:
                    OnTakeShield(GetDescriptorByType(objectType));
                    break;
            }
        }

        private void OnTakeShield(BoosterDescriptor shieldBoosterDescriptor)
        {
            _shieldBoosterDescriptor = shieldBoosterDescriptor;
            Debug.Log(shieldBoosterDescriptor.Id);
            _droneAnimService.PlayAnimState(DroneAnimState.amEnableShield);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SHIELD));
            Invoke(nameof(DisableShield), float.Parse(_shieldBoosterDescriptor.Params["Duration"]));
        }

        private void DisableShield()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SHIELD));
            _droneAnimService.PlayAnimState(DroneAnimState.amDisableShield);
        }

        private void OnTakeSpeed(BoosterDescriptor speedBoosterDescriptor)
        {
            _speedBoosterDescriptor = speedBoosterDescriptor;
            Debug.Log(_speedBoosterDescriptor.Id);
            _droneModel.energy -= float.Parse(_speedBoosterDescriptor.Params["NeedsEnergy"]);
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SPEED, _speedBoosterDescriptor));
            _droneAnimService.PlayAnimState(DroneAnimState.amEnableSpeed);
            Invoke(nameof(DisableSpeed), float.Parse(_speedBoosterDescriptor.Params["Duration"]));
        }
        
        private void DisableSpeed()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SPEED, _speedBoosterDescriptor));
            _droneAnimService.PlayAnimState(DroneAnimState.amDisableSpeed);
        }
    }
}