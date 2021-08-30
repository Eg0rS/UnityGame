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
        public const string CREATE_WORLD = "CreateWorld";
        public const string SWIPE = "Swipe";

        public GameObject CollisionObject { get; private set; }
        public LevelDescriptor LevelDescriptor { get; private set; }
        public DronStats DronStats { get; private set; }
        public WorldObjectType TypeBoost { get; private set; }
        public string DronId { get; private set; }
        public float SpeedBoost { get; private set; }
        public float SpeedBoostTime { get; private set; }
        public Vector2 Swipe { get; private set; }
        
        public WorldEvent(string name, GameObject target) : base(name, target)
        {
            CollisionObject = target;
        }

        public WorldEvent(string name, DronStats dronStats) : base(name)
        {
            DronStats = dronStats;
        }

        public WorldEvent(string name, WorldObjectType type) : base(name)
        {
            TypeBoost = type;
        }

        public WorldEvent(string name, float speedBoost, float speedBoostTime) : base(name)
        {
            SpeedBoost = speedBoost;
            SpeedBoostTime = speedBoostTime;
        }

        public WorldEvent(string name, LevelDescriptor levelDescriptor, string dronId) : base(name)
        {
            LevelDescriptor = levelDescriptor;
            DronId = dronId;
        }
        
        public WorldEvent(string name, Vector2 swipe) : base(name)
        {
            Swipe =swipe ;
        }

        public WorldEvent(string name) : base(name)
        {
        }

        public T GetController<T>()
        {
            return Target.GetComponent<T>();
        }
    }
}