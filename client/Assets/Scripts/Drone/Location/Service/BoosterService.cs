using System;
using System.Collections;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.World.BonusChips;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.Service
{
    public class BoosterService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private ResourceService _resourceService;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private const float TIME_SCAN_FOR_CHIPS = 0.2f;

        public bool IsShieldActivate { get; private set; }
        public bool IsX2Activate { get; private set; }

        private Coroutine _magnetCoroutine;

        private Dictionary<string, BoosterDescriptor> _boosterDescriptors;

        public void Init()
        {
            _boosterDescriptors = new Dictionary<string, BoosterDescriptor>();
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SPEED, OnTakeSpeed);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_SHIELD, OnTakeShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_X2, OnTakeX2);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.TAKE_MAGNET, OnTakeMagnet);
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
        private BoosterDescriptor GetDescriptorByType(WorldObjectType objectType)
        {
            string type = Enum.GetName(typeof(WorldObjectType), objectType);
            return _boosterDescriptors[type];
        }

        private float GetDescriptorParametr(WorldObjectType objectType, string param)
        {
            return float.Parse(GetDescriptorByType(objectType).Params[param]);
        }

        private void OnTakeSpeed(WorldEvent worldEvent)
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SPEED, GetDescriptorByType(WorldObjectType.SPEED_BOOSTER)));
            Invoke(nameof(DisableSpeed), GetDescriptorParametr(WorldObjectType.SPEED_BOOSTER, "Duration"));
        }

        private void DisableSpeed()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SPEED, GetDescriptorByType(WorldObjectType.SPEED_BOOSTER)));
        }

        private void OnTakeShield(WorldEvent worldEvent)
        {
            IsShieldActivate = true;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SHIELD));
            Invoke(nameof(DisableShield), GetDescriptorParametr(WorldObjectType.SHIELD_BOOSTER, "Duration"));
        }

        private void DisableShield()
        {
            IsShieldActivate = false;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SHIELD));
        }

        private void OnTakeX2(WorldEvent obj)
        {
            IsX2Activate = true;
            Invoke(nameof(DisableX2), GetDescriptorParametr(WorldObjectType.X2_BOOSTER, "Duration"));
        }

        private void DisableX2()
        {
            IsX2Activate = false;
        }

        private void OnTakeMagnet(WorldEvent worldEvent)
        {
            _magnetCoroutine = StartCoroutine(ScanForChips(worldEvent.Drone));
            Invoke(nameof(DisableMagnet), GetDescriptorParametr(WorldObjectType.MAGNET_BOOSTER, "Duration"));
        }

        private IEnumerator ScanForChips(GameObject drone)
        {
            while (true) {
                Collider[] colliders = Physics.OverlapSphere(drone.transform.position, GetDescriptorParametr(WorldObjectType.MAGNET_BOOSTER, "Radius"));
                foreach (Collider collider in colliders) {
                    PrefabModel model = collider.gameObject.GetComponent<PrefabModel>();
                    if (model != null && model.ObjectType == WorldObjectType.BONUS_CHIPS) {
                        collider.gameObject.GetComponent<BonusChipsController>().MoveToDrone(drone.transform.position);
                    }
                }
                yield return new WaitForSeconds(TIME_SCAN_FOR_CHIPS);
            }
        }

        private void DisableMagnet()
        {
            StopCoroutine(_magnetCoroutine);
        }
    }
}