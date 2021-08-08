using AgkCommons.Event;
using DronDonDon.Location.Model;
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
        public const string SHIELD_ACTIVATE = "ShieldActive";
        public const string SPEED_ACTIVE = "SpeedActive";
        public const string STOP_GAME = "StopGame";
        
        public WorldObjectEvent(string name, GameObject target) : base(name, target)
        {
            
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