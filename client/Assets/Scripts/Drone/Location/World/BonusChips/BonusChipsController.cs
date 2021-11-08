using System;
using System.Collections;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.BonusChips;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.BonusChips
{
    public class BonusChipsController : MonoBehaviour, IWorldObjectController<BonusChipsModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        private float _speedForMagnetic = 3f;
        private float _coefIncreaseSpeed = 3f;

        private bool _isCollected;
        private bool _isMagnetic;
        private Vector3 _dronePosition;
        private Vector3 _startPosition;
        private float _coefCompleteWay;

        public void Init(BonusChipsModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _isCollected = true;
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_CHIP));
            }
        }

        public void Update()
        {
            if (_isCollected || !_isMagnetic) {
                return;
            }
            _coefCompleteWay += _speedForMagnetic * Time.deltaTime;
            transform.position = Vector3.Lerp(_startPosition, _dronePosition, _coefCompleteWay);
        }

        public void MoveToDrone(Vector3 position)
        {
            if (_isCollected) {
                return;
            }
            _dronePosition = position;
            _startPosition = transform.position;
            _coefCompleteWay = 0;
            if (_isMagnetic) {
                _speedForMagnetic *= _coefIncreaseSpeed;
            } else {
                _isMagnetic = true;
            }
        }
    }
}