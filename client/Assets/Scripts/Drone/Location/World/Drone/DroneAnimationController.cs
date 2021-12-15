using System;
using AgkCommons.Event;
using Drone.Location.Event;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Extension;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.Location.Service.Control.Drone
{
    public class DroneAnimationController : GameEventDispatcher
    {
        [InjectComponent]
        private Animator _animator;
        private const float LACK_OF_SPEED = 0f;
        private float _animSpeed = 1f;

        private void Awake()
        {
            this.InjectComponents();
        }

        public void OnCrash(ObstacleEvent obstacleEvent)
        {
            foreach (ContactPoint contactPoint in obstacleEvent.ContactPoints) {
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

        private void PlayParticleState(DroneParticles particle, Vector3 position, Quaternion rotation)
        {
            Instantiate(Resources.Load<GameObject>("Embeded/Particles/" + GetParticleName(particle)), position, rotation);
        }
    }
}