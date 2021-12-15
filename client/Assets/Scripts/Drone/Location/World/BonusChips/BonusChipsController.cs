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

        private float _speedForMagnetic = 10f;

        private bool _isCollected;
        private bool _isMagnetic;
        private Transform _droneTransform;

        public void Init(BonusChipsModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.PLAYER) {
                gameObject.SetActive(false);
                _isCollected = true;
                _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_CHIP));
            }
        }

        private void Update()
        {
            if (_isMagnetic && !_isCollected) {
                transform.position = Vector3.MoveTowards(transform.position, _droneTransform.position, _speedForMagnetic * Time.deltaTime);
            }
        }

        public void MoveToDrone(Transform droneTransform)
        {
            _isMagnetic = true;
            _droneTransform = droneTransform;
        }
    }
}