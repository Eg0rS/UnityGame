using AgkCommons.Event;
using JetBrains.Annotations;
using UnityEngine;

namespace DronDonDon.World.Event
{
    [PublicAPI]
    public class WorldObjectEvent : GameEvent
    {
        public const string ADDED = "WorldObjectAdded";
        public const string CHANGED = "WorldObjectChanged";
        public const string SELECTED = "WorldObjectSelected";
        public const string ON_COLLISION = "OnCollision";
        public const string UI_UPDATE = "UiUpdate";

        public GameObject _collisionObject;
        public DronStats _dronStats;
        public WorldObjectEvent(string name, GameObject target) : base(name, target)
        {
            _collisionObject = target;
        }
        
        public WorldObjectEvent(string name, DronStats dronStats) : base(name)
        {
            _dronStats = dronStats;
        }

        public WorldObjectEvent(string name) : base(name)
        {
            
        }

        public T GetController<T>()
        {
            return Target.GetComponent<T>();
        }
    }
}