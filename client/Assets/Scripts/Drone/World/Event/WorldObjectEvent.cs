using AgkCommons.Event;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.World.Event
{
    [PublicAPI]
    public class WorldObjectEvent : GameEvent
    {
        public const string ADDED = "WorldObjectAdded";
        public const string CHANGED = "WorldObjectChanged";
        public const string SELECTED = "WorldObjectSelected";
        public const string WORLD_CREATED = "WorldCreated";

        public const string END_GAME = "EndGame";
        public const string FINISHED = "Finished";

        public const string ENABLE_SHIELD = "EnableShield";
        public const string DISABLE_SHIELD = "DisableShield";
        public const string TAKE_CHIP = "TakeChip";
        public const string TAKE_SHIELD = "TakeShield";
        public const string TAKE_X2 = "TakeX2";
        public const string TAKE_MAGNET = "TakeMagnet";

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