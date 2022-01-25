using AgkCommons.Event;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.World.Event
{
    [PublicAPI]
    public class WorldObjectEvent : GameEvent
    {
        public const string ADDED = "WorldObjectAdded";
        public const string CHANGED = "WorldObjectChanged";
        public const string SELECTED = "WorldObjectSelected";
        

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