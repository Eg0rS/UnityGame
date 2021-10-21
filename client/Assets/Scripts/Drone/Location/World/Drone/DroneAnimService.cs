using System;
using Drone.World;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimService : MonoBehaviour
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private const float NULL_SPEED = -1f;

        private const float NORMILIZED_BLEND_ANIM_MOVE = 0.15f;

        private Animator _animator;

        private float _animSpeed = 1f;

        private DroneAnimState _lastDroneAnimMoveState = DroneAnimState.amIdle;

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