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

        private const float NORMILIZED_BLEND_ANIM_MOVE = 0.15f;

        private Animator _animator;

        private float _animSpeed = 1;

        private AnimState _lastAnimMoveState = AnimState.amIdle;

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
            List<GameObject> list = _gameWorld.Require().GetGameObjectByName("DronCube").GetChildren(); //todo оптимизировать
            GameObject droneModel = list[0];
            _animator = droneModel.GetComponentInChildren<Animator>();
        }

        public void SetAnimState(AnimState animState, float speed = -1)
        {
            if (speed.Equals(-1)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            _animator.Play(Enum.GetName(typeof(AnimState), animState));
        }

        public void SetAnimMoveState(AnimState animMoveState, float speed)
        {
            _animator.speed = speed;
            if (animMoveState != _lastAnimMoveState) {
                _animator.CrossFade(Enum.GetName(typeof(AnimState), animMoveState), NORMILIZED_BLEND_ANIM_MOVE);
            } else {
                _animator.CrossFade(Enum.GetName(typeof(AnimState), AnimState.amIdle), NORMILIZED_BLEND_ANIM_MOVE * 2);
                _animator.Play(Enum.GetName(typeof(AnimState), animMoveState), 0,
                               NORMILIZED_BLEND_ANIM_MOVE); //todo исправить проблему с резким переключением при небольшой скорости
            }
            _lastAnimMoveState = animMoveState;
        }
    }
}