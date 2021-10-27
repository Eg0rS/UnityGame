using System;
using AgkCommons.Event;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Extension;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneAnimationController : GameEventDispatcher
    {
        [InjectComponent]
        private Animator _animator;
        private const float LACK_OF_SPEED = 0f;
        private const float NORMILIZED_BLEND_ANIM_MOVE = 0.15f;
        private float _animSpeed = 1f;
        private DroneAnimState _lastDroneAnimMoveState = DroneAnimState.amIdle;

        private void Awake()
        {
            this.InjectComponents();
        }

        public void OnCrash(WorldEvent worldEvent)
        {
            foreach (ContactPoint contactPoint in worldEvent.ContactPoints) {
                PlayParticleState(DroneParticles.ptSparks, contactPoint.point, transform.rotation);
            }
        }
        
        public void OnCrashed()
        {
            PlayParticleState(DroneParticles.ptExplosion, transform.position, transform.rotation);
        }
        
        public void EnableSpeedBoost()
        {
            PlayAnimState(DroneAnimState.amEnableSpeed);
        }
        
        public void DisableSpeedBoost()
        {
            PlayAnimState(DroneAnimState.amDisableSpeed);
        }

        public void EnableShield()
        {
            PlayAnimState(DroneAnimState.amEnableShield);
        }
        
        public void DisableShield()
        {
            PlayAnimState(DroneAnimState.amDisableShield);
        }
        
        public float DefaultAnimSpeed
        {
            get { return _animSpeed; }
            set { _animSpeed = value; }
        }

        public void MoveTo(Vector3 direction, Vector3 pos, float mobility)
        {
            SetAnimMoveState(DetectDirection(direction), CalculateAnimSpeed(pos, mobility));
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

        private void PlayAnimState(DroneAnimState droneAnimState, float speed = LACK_OF_SPEED)
        {
            if (speed.Equals(LACK_OF_SPEED)) {
                speed = DefaultAnimSpeed;
            }
            _animator.speed = speed;
            _animator.Play(GetAnimName(droneAnimState));
        }

        private void SetAnimMoveState(DroneAnimState droneAnimMoveState, float speed = LACK_OF_SPEED)
        {
            if (speed.Equals(LACK_OF_SPEED)) {
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

        private void PlayParticleState(DroneParticles particle, Vector3 position, Quaternion rotation)
        {
            Instantiate(Resources.Load<GameObject>("Embeded/Particles/" + GetParticleName(particle)), position, rotation);
        }

        private DroneAnimState DetectDirection(Vector3 vector)
        {
            if (vector.Equals(Vector3.right)) {
                return DroneAnimState.amMoveRight;
            }
            if (vector.Equals(Vector3.left)) {
                return DroneAnimState.amMoveLeft;
            }
            if (vector.Equals(Vector3.up)) {
                return DroneAnimState.amMoveUp;
            }
            if (vector.Equals(Vector3.down)) {
                return DroneAnimState.amMoveDown;
            }
            if (vector.Equals(new Vector3(1, 1, 0))) {
                return DroneAnimState.amMoveUpRight;
            }
            if (vector.Equals(new Vector3(-1, 1, 0))) {
                return DroneAnimState.amMoveUpLeft;
            }
            if (vector.Equals(new Vector3(1, -1, 0))) {
                return DroneAnimState.amMoveDownRight;
            }
            if (vector.Equals(new Vector3(-1, -1, 0))) {
                return DroneAnimState.amMoveDownLeft;
            }
            if (vector.Equals(Vector3.zero)) {
                return DroneAnimState.amIdle;
            }
            throw new Exception("Incorrect vector for animation: " + vector);
        }

        private float CalculateAnimSpeed(Vector3 newPos, float mobility)
        {
            return 0.3f * (1 / ((1 / (mobility * 10)) * (newPos - transform.localPosition).magnitude));
        }
    }
}