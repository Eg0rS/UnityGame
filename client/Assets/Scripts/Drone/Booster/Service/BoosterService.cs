using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Booster.Descriptor;
using Drone.Core.Filter;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Service;
using Drone.Location.World.Dron.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Booster.Service
{
    public class BoosterService : MonoBehaviour, IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        [Inject]
        private GameService _gameService;

        private List<BoosterDescriptor> _boosterDescriptors;
        private bool _onActiveShield;
        private DroneModel _droneModel;

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
            Debug.Log("a");
            throw new Exception("BoosterDescriptor not found");
        }

        private void OnWorldCreated(WorldEvent objectEvent)
        {
            _droneModel = objectEvent.DroneModel;
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
        }

        private void OnDronCollision(WorldEvent worldEvent)
        {
            WorldObjectType objectType = worldEvent.CollisionObject.GetComponent<PrefabModel>().ObjectType;
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
            _onActiveShield = true;
            Invoke(nameof(DisableShield), (float) shieldBoosterDescriptor.Params["Duration"]);
        }

        private void OnTakeSpeed(BoosterDescriptor speedBoosterDescriptor)
        {
            _droneModel.energy -= (float) speedBoosterDescriptor.Params["NeedsEnergy"];
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRON_BOOST_SPEED, speedBoosterDescriptor));
        }

        private void DisableShield()
        {
            _onActiveShield = false;
        }
    }
}