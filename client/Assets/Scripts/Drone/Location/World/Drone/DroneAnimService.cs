using System;
using AgkCommons.Event;
using Drone.Core.Filter;
using Drone.Location.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimService: GameEventDispatcher, IInitable
    {
        [Inject]
        private GameService _gameService;
        
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private Animator _animator;
        
        public void Init()
        {
            _gameService.AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnWorldCreated(WorldEvent obj)
        {
            _animator = _gameWorld.Require().GetGameObjectByName("DronCube")?.GetComponentInChildren<Animator>();
        }

        public void SetAnimState(AnimState animState)
        {
            _animator.Play(Enum.GetName(typeof(AnimState), animState));
        }
    }
}