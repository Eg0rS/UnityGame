using System;
using AgkCommons.Event;
using Drone.Core.Filter;
using Drone.Location.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimController : GameEventDispatcher, IInitable
    {
        [Inject]
        private GameService _gameService;

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        private const float NULL_SPEED = -1f;

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
            _animator = _gameWorld.Require().GetDroneAnimator();
        }

        [CanBeNull]
        private string GetAnimName(DroneAnimState droneAnimState)
        {
            return Enum.GetName(typeof(DroneAnimState), droneAnimState);
        }

        public void PlayAnimState(DroneAnimState droneAnimState, float speed = NULL_SPEED)
        {
            if (speed.Equals(NULL_SPEED)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            _animator.Play(GetAnimName(droneAnimState));
        }

        public void SetAnimMoveState(DroneAnimState droneAnimMoveState, float speed = -1)
        {
            if (speed.Equals(-1)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            if (droneAnimMoveState != _lastDroneAnimMoveState) {
                _animator.CrossFade(GetAnimName(droneAnimMoveState), NORMILIZED_BLEND_ANIM_MOVE);
            } else {
                _animator.CrossFade(GetAnimName(DroneAnimState.amIdle), NORMILIZED_BLEND_ANIM_MOVE * 2);
                _animator.Play(GetAnimName(droneAnimMoveState), 0, NORMILIZED_BLEND_ANIM_MOVE);
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