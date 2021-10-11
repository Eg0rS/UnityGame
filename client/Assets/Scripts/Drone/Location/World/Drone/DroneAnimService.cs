using System;
using System.Collections.Generic;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.Core.Filter;
using Drone.Location.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimService : GameEventDispatcher, IInitable
    {
        [Inject]
        private GameService _gameService;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private Animator _animator;

        public void Init()
        {
            _gameService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            _gameService.AddListener<WorldEvent>(WorldEvent.END_GAME, OnWorldDestroy);
        }

        private void OnWorldCreated(WorldEvent obj)
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
        }
        
        private void OnWorldDestroy(WorldEvent obj)
        {
            _gameWorld.Require().RemoveListener<WorldEvent>(WorldEvent.START_FLIGHT, StartGame);
        }

        private void StartGame(WorldEvent obj)
        {
            List<GameObject> list = _gameWorld.Require().GetGameObjectByName("DronCube").GetChildren(); //todo оптимизировать
            GameObject droneModel = list[0];
            _animator = droneModel.GetComponentInChildren<Animator>();
        }
        

        public void SetAnimState(AnimState animState)
        {
            _animator.Play(Enum.GetName(typeof(AnimState), animState));
        }
    }
}