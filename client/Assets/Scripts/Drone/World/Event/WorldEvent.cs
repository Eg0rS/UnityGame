﻿using AgkCommons.Event;
using Drone.Descriptor;
using Drone.Location.Service;
using Drone.Location.World.Drone;
using Drone.Location.World.Drone.Model;
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
        public const string UI_UPDATE = "UiUpdate";
        public const string START_FLIGHT = "StartFlight";
        public const string END_GAME = "EndGame";
        public const string FINISHED = "Finished";
        public const string WORLD_CREATED = "WorldCreated";
        public const string SET_DRON_PARAMETERS = "SetDronParameters";
        public const string CRASH = "Crash";
        public const string DRONE_CRASH = "DroneCrash";
        public const string CRASHED = "Crashed";
        public const string PLAY_ANIMATE = "PlayAnimate";
        public const string ENABLE_SHIELD = "EnableShield";
        public const string DISABLE_SHIELD = "DisableShield";
        public const string ENABLE_SPEED = "EnableSpeed";
        public const string DISABLE_SPEED = "DisableSpeed";
        public const string TAKE_CHIP = "TakeChip";
        public const string TAKE_BATTERY = "TakeBattery";
        public const string TAKE_SPEED = "TakeSpeed";
        public const string TAKE_SHIELD = "TakeShield";

        public DroneModel DroneModel { get; private set; }
        public BoosterDescriptor SpeedBooster { get; private set; }
        public FailedReasons FailedReason { get; private set; }
        public DroneAnimState DroneAnimState { get; private set; }
        public DroneParticles DroneParticles { get; private set; }
        public ContactPoint[] ContactPoints { get; private set; }
        public Transform Transform { get; private set; }
        public float ImmersionDepth { get; private set; }

        public WorldEvent(string name, GameObject target) : base(name, target)
        {
        }

        public WorldEvent(string name, DroneAnimState state) : base(name)
        {
            DroneAnimState = state;
        }

        public WorldEvent(string name, ContactPoint[] contactPoints, float immersionDepth) : base(name)
        {
            ContactPoints = contactPoints;
            ImmersionDepth = immersionDepth;
        }

        public WorldEvent(string name, DroneModel droneModel) : base(name)
        {
            DroneModel = droneModel;
        }

        public WorldEvent(string name, BoosterDescriptor speedBooster) : base(name)
        {
            SpeedBooster = speedBooster;
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