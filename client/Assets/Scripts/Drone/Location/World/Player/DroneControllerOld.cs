using System.Collections;
using AgkCommons.Event;
using AgkCommons.Extension;
using BezierSolution;
using Cinemachine;
using DG.Tweening;
using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.Player;
using Drone.Location.Service.Control.Drone.Event;
using Drone.Location.Service.Game.Event;
//using Drone.World;
using GameKit.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.World.Drone
{
    public class DroneControllerOld : GameEventDispatcher
    {
        public WorldObjectType ObjectType { get; }
        private const float MINIMAL_SPEED = 3.0f;
        private const float CRASH_NOISE = 2;
        private const float CRASH_NOISE_DURATION = 0.5f;

        [Inject]
        private DroneWorld _gameWorld;

        private BezierWalkerWithSpeed _bezier;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        private float _acceleration;
        private float _maxSpeed;
        private float _baseMobility;
        private float _mobility;

        private Vector3 _droneTargetPosition = Vector3.zero;
        private bool _isGameRun;
        private Sequence _sequence;

        public void Init(PlayerModel model)
        {
            _bezier = transform.parent.transform.GetComponentInParent<BezierWalkerWithSpeed>();
            //_cameraNoise = _gameWorld.GetDroneCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);

            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
            _gameWorld.AddListener<ControllEvent>(ControllEvent.GESTURE, OnGesture);

            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT_BEGIN, OnCrush);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.END_GAME, OnEndGame);

            // _gameWorld.AddListener<AcceleratorEvent>(AcceleratorEvent.ACCELERATION, OnAccelerator);

            _sequence = DOTween.Sequence();
            _sequence.SetUpdate(UpdateType.Fixed);
        }

        private void OnEndGame(InGameEvent inGameEvent)
        {
            EndGameReasons reason = inGameEvent.EndGameReason;
            switch (reason) {
                case EndGameReasons.OUT_OF_ENERGY:
                    break;
                case EndGameReasons.OUT_OF_DURABILITY:
                    DroneCrash();
                    break;
            }
        }

        private void OnCrush(ObstacleEvent obstacleEvent)
        {
            _bezier.speed /= 2;
            _cameraNoise.m_AmplitudeGain += CRASH_NOISE;
            Invoke(nameof(DisableCrashNoise), CRASH_NOISE_DURATION);
        }

        // private void OnAccelerator(AcceleratorEvent acceleratorEvent)
        // {
        //     StartCoroutine(AcceleratorCorutine(acceleratorEvent.AcceleratorModel.AccelerationDuration,
        //                                        acceleratorEvent.AcceleratorModel.AccelerationForce));
        // }

        private IEnumerator AcceleratorCorutine(float duration, float force)
        {
            _maxSpeed *= force;
            _acceleration *= force;

            yield return new WaitForSeconds(duration);

            _maxSpeed /= force;
            _acceleration /= force;
        }

        private void OnGesture(ControllEvent objectEvent)
        {
            Vector3 swipe = new Vector3(objectEvent.Gesture.x, objectEvent.Gesture.y, 0f);
            Vector3 newPosition = NewPosition(_droneTargetPosition, swipe);
            if (_droneTargetPosition.Equals(newPosition)) {
                return;
            }
            _gameWorld.Dispatch(new ControllEvent(ControllEvent.MOVEMENT, newPosition));
            DotWeenMove(newPosition);
        }

        private Vector3 NewPosition(Vector3 dronPos, Vector3 swipe)
        {
            Vector3 newPos = dronPos + swipe;
            if (newPos.x > 1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.x < -1.0f) {
                swipe.x = 0.0f;
            }
            if (newPos.y > 1.0f) {
                swipe.y = 0.0f;
            }
            if (newPos.y < -1.0f) {
                swipe.y = 0.0f;
            }
            Vector3 newPosition = dronPos + swipe;
            return newPosition;
        }

        private void DotWeenMove(Vector3 newPos)
        {
            _mobility = _baseMobility * (MINIMAL_SPEED / _bezier.speed);
            Vector3 rotation = new Vector3(_droneTargetPosition.y - newPos.y, transform.localRotation.y, _droneTargetPosition.x - newPos.x) * 30;
            _droneTargetPosition = newPos;
            _sequence.Append(transform.DOLocalMove(newPos, _mobility).SetUpdate(UpdateType.Fixed))
                     .Join(transform.DOLocalRotate(rotation, _mobility)
                                    .SetUpdate(UpdateType.Fixed)
                                    .OnComplete(() => { transform.DOLocalRotate(Vector3.zero, _mobility).SetUpdate(UpdateType.Fixed); }));
        }

        private void OnSetParameters(InGameEvent inGameEvent)
        {
            _bezier.speed = 0;
            _maxSpeed = inGameEvent.DroneModel.maxSpeed;
            _acceleration = inGameEvent.DroneModel.acceleration;
            _baseMobility = inGameEvent.DroneModel.mobility;
            CreateDrone(inGameEvent.DroneModel.DroneDescriptor.Prefab);
        }

        private void CreateDrone(string droneDescriptorPrefab)
        {
            GameObject drone = Instantiate(Resources.Load<GameObject>(droneDescriptorPrefab));
            _gameWorld.AddGameObject(drone, gameObject.GetChildren().Find(x => x.name == "Mesh"));
        }

        private void OnStartGame(InGameEvent inGameEvent)
        {
            _bezier.speed = MINIMAL_SPEED;
            _isGameRun = true;
        }

        public void Update()
        {
            if (!_isGameRun) {
                return;
            }
            SetBezierSpeed();
        }

        private void SetBezierSpeed()
        {
            if (_bezier.speed >= _maxSpeed) {
                return;
            }
            float newSpeed = _bezier.speed + _acceleration * Time.deltaTime;
            _bezier.speed = newSpeed > _maxSpeed ? _maxSpeed : newSpeed;
        }

        private void DroneCrash()
        {
            _bezier.enabled = false;
            gameObject.SetActive(false);
            _cameraNoise.m_AmplitudeGain += CRASH_NOISE;
            Invoke(nameof(DisableCrashNoise), CRASH_NOISE_DURATION);
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= CRASH_NOISE;
        }
    }
}