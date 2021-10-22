using System;
using AgkCommons.Event;
using AgkCommons.Extension;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Extension;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimService : GameEventDispatcher
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private const float NULL_SPEED = -1f;

        private const float NORMILIZED_BLEND_ANIM_MOVE = 0.15f;
        
        private Animator _animator;

        private float _animSpeed = 1f;

        private DroneAnimState _lastDroneAnimMoveState = DroneAnimState.amIdle;

        //   private void Awake()
        //   {
        //       //_animator = _gameWorld.Require().GetDroneAnimator();
        //       _animator= gameObject.GetChildren().Find(x => x.name.Equals("pfDroneBase1(Clone)")).GetComponentInChildren<Animator>();
        //        // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.PLAY_ANIMATE, OnPlayAnimation);
        // // //     // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, OnCrush);
        //   }
        //
        //  private void Start()
        //  {
        //      _animator = _gameWorld.Require().GetDroneAnimator();
        //
        // //     // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.PLAY_ANIMATE, OnPlayAnimation);
        // //     // _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.CRASH, OnCrush);
        //  }

        public void OnCrush(WorldEvent worldEvent)
        {
            PlayParticleState(worldEvent.DroneParticles, worldEvent.ContactPoint.point, worldEvent.Transform.rotation);
        }

        public void OnPlayAnimation(WorldEvent worldEvent)
        {
            PlayAnimState(worldEvent.DroneAnimState);
        }
        public float DefaultAnimSpeed
        {
            get { return _animSpeed; }
            set { _animSpeed = value; }
        }

        
        private void LoadAnimator()
        {
            _animator = _gameWorld.Require().GetDroneAnimator();
        }

        [CanBeNull]
        private string GetAnimName(DroneAnimState droneAnimState)
        {
            return Enum.GetName(typeof(DroneAnimState), droneAnimState);
        }

        [CanBeNull]
        private string GetParticleName(DroneParticles particle)
        {
            return Enum.GetName(typeof(DroneParticles), particle);
        }

        public void PlayAnimState(DroneAnimState droneAnimState, float speed = NULL_SPEED)
        {
            if (_animator == null) {
                LoadAnimator();
            }
            if (speed.Equals(NULL_SPEED)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            _animator.Play(GetAnimName(droneAnimState));
        }

        public void SetAnimMoveState(DroneAnimState droneAnimMoveState, float speed = -1)
        {
            if (_animator == null) {
                LoadAnimator();
            }
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
            Instantiate(Resources.Load<GameObject>("Embeded/Particles/" + GetParticleName(particle)), position, rotation);
        }
    }
}