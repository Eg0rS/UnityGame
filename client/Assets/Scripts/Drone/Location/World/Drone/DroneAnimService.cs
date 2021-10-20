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
using Unity.Mathematics;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimService : GameEventDispatcher, IInitable
    {
        [Inject]
        private GameService _gameService;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private const float NORMILIZED_BLEND_ANIM_MOVE = 0.15f;

        private Animator _animator;

        private float _animSpeed = 1;

        private DroneAnimState _lastDroneAnimMoveState = DroneAnimState.amIdle;

        public float DefaultAnimSpeed
        {
            get { return _animSpeed; }
            set { _animSpeed = value; }
        }

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
            GameObject droneModel = _gameWorld.Require().GetGameObjectByName("pfDroneModelContainer"); //todo оптимизировать
            _animator = droneModel.GetComponentInChildren<Animator>();
        }

        public void PlayAnimState(DroneAnimState droneAnimState, float speed = -1)
        {
            if (speed.Equals(-1)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            _animator.Play(Enum.GetName(typeof(DroneAnimState), droneAnimState));
        }

        public void SetAnimMoveState(DroneAnimState droneAnimMoveState, float speed = -1)
        {
            if (speed.Equals(-1)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            if (droneAnimMoveState != _lastDroneAnimMoveState) {
                _animator.CrossFade(Enum.GetName(typeof(DroneAnimState), droneAnimMoveState), NORMILIZED_BLEND_ANIM_MOVE);
            } else {
                _animator.CrossFade(Enum.GetName(typeof(DroneAnimState), DroneAnimState.amIdle), NORMILIZED_BLEND_ANIM_MOVE * 2);
                _animator.Play(Enum.GetName(typeof(DroneAnimState), droneAnimMoveState), 0, NORMILIZED_BLEND_ANIM_MOVE);
                //todo исправить проблему с резким переключением при небольшой скорости
            }
            _lastDroneAnimMoveState = droneAnimMoveState;
        }

        public void PlayParticleState(DroneParticles particle, Vector3 position, Quaternion rotation)
        {
            Instantiate(Resources.Load<GameObject>("Embeded/Particles/" + Enum.GetName(typeof(DroneParticles), particle)), position, rotation);
        }
    }
}