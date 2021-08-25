using AgkCommons.Event;
using DeliveryRush.Location.Model;
using DeliveryRush.Resource.Descriptor;
using JetBrains.Annotations;
using UnityEngine;

namespace DeliveryRush.World.Event
{
    [PublicAPI]
    public class WorldEvent : GameEvent
    {
        public const string ADDED = "WorldObjectAdded";
        public const string CHANGED = "WorldObjectChanged";
        public const string SELECTED = "WorldObjectSelected";
        public const string ON_COLLISION = "OnCollision";
        public const string UI_UPDATE = "UiUpdate";
        public const string ACTIVATE_BOOST = "ActivateBoost";
        public const string TAKE_BOOST = "TakeBoost";
        public const string START_GAME = "StartGame";
        public const string END_GAME = "EndGame";
        public const string DRON_BOOST_SPEED = "DronBoostSpeed";
        public const string WORLD_CREATED = "WorldCreated";

        public GameObject _collisionObject;
        public DronStats _dronStats;
        public WorldObjectType _typeBoost;
        public float SpeedBoost;
        public float SpeedBoostTime;

        // public LevelDescriptor LevelDescriptor { get; set; }
        // public string DronId { get; set; }

        public WorldEvent(string name, GameObject target) : base(name, target)
        {
            _collisionObject = target;
        }

        public WorldEvent(string name, DronStats dronStats) : base(name)
        {
            _dronStats = dronStats;
        }

        public WorldEvent(string name, WorldObjectType type) : base(name)
        {
            _typeBoost = type;
        }

        public WorldEvent(string name, float speedBoost, float speedBoostTime) : base(name)
        {
            SpeedBoost = speedBoost;
            SpeedBoostTime = speedBoostTime;
        }

        // public WorldEvent(string name, LevelDescriptor levelDescriptor, string dronId) : base(name)
        // {
        //     LevelDescriptor = levelDescriptor;
        //     DronId = dronId;
        // }

        public WorldEvent(string name) : base(name)
        {
        }

        public T GetController<T>()
        {
            return Target.GetComponent<T>();
        }
    }
}