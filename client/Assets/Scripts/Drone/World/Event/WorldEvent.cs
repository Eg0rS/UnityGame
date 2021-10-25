using AgkCommons.Event;
using Drone.Descriptor;
using Drone.LevelMap.Levels.Descriptor;
using Drone.Location.Model;
using Drone.Location.Service;
using Drone.Location.World.Drone;
using Drone.Location.World.Drone.Model;
using Drone.Settings.Service;
using JetBrains.Annotations;
using UnityEngine;

namespace Drone.World.Event
{
    [PublicAPI]
    public class WorldEvent : GameEvent
    {
        public const string ADDED = "WorldObjectAdded";
        public const string CHANGED = "WorldObjectChanged";
        public const string SELECTED = "WorldObjectSelected";
        public const string ON_COLLISION = "OnCollision";
        public const string UI_UPDATE = "UiUpdate";
        public const string ENABLE_SHIELD = "EnableShield";
        public const string DISABLE_SHIELD = "DisableShield";
        public const string ENABLE_SPEED = "EnableSpeed";
        public const string DISABLE_SPEED = "DisableSpeed";
        public const string TAKE_BOOST = "TakeBoost";
        public const string START_FLIGHT = "StartFlight";
        public const string DRONE_FAILED = "DroneFailed";
        public const string END_GAME = "EndGame";
        public const string WORLD_CREATED = "WorldCreated";
        public const string SET_DRON_PARAMETERS = "SetDronOptions";
        public const string CREATE_WORLD = "CreateWorld";
        public const string CRASH = "Crash";
        public const string PLAY_ANIMATE = "PlayAnimate";
        public const string TAKE_CHIP = "TakeChip";
        public const string TAKE_BATTERY = "TakeBattery";

        public Collision CollisionObject { get; private set; }
        public LevelDescriptor LevelDescriptor { get; private set; }
        public DroneModel DroneModel { get; private set; }
        public WorldObjectType TypeBoost { get; private set; }
        public string DronId { get; private set; }
        public BoosterDescriptor SpeedBooster { get; private set; }
        public FailedReasons FailedReason { get; private set; }
        public DroneAnimState DroneAnimState { get; private set; }
        public DroneParticles DroneParticles { get; private set; }
        public ContactPoint ContactPoint { get; private set; }
        public Transform Transform { get; private set; }

        public WorldEvent(string name, GameObject target) : base(name, target)
        {
        }

        public WorldEvent(string name, DroneAnimState state) : base(name)
        {
            DroneAnimState = state;
        }
        public WorldEvent(string name, DroneParticles particles, ContactPoint contactPoint, Transform transform) : base(name)
        {
            DroneParticles = particles;
            ContactPoint = contactPoint;
            Transform = transform;
        }

        public WorldEvent(string name, Collision target) : base(name)
        {
            CollisionObject = target;
        }

        public WorldEvent(string name, DroneModel droneModel) : base(name)
        {
            DroneModel = droneModel;
        }

        public WorldEvent(string name, WorldObjectType type) : base(name)
        {
            TypeBoost = type;
        }

        public WorldEvent(string name, BoosterDescriptor speedBooster) : base(name)
        {
            SpeedBooster = speedBooster;
        }
        

        public WorldEvent(string name) : base(name)
        {
        }

        public WorldEvent(string name, FailedReasons failedReason) : base(name)
        {
            FailedReason = failedReason;
        }

        public T GetController<T>()
        {
            return Target.GetComponent<T>();
        }
    }
}