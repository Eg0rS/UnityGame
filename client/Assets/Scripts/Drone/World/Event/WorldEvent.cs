﻿using System;
using AgkCommons.Event;
using Drone.LevelMap.Levels.Descriptor;
using Drone.Location.Model;
using Drone.Location.Service;
using Drone.Location.World.Dron.Model;
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
        public const string ACTIVATE_BOOST = "ActivateBoost";
        public const string TAKE_BOOST = "TakeBoost";
        public const string START_FLIGHT = "StartFlight";
        public const string END_GAME = "EndGame";
        public const string DRON_BOOST_SPEED = "DronBoostSpeed";
        public const string WORLD_CREATED = "WorldCreated";
        public const string SET_DRON_PARAMETERS = "SetDronOptions";
        public const string CREATE_WORLD = "CreateWorld";

        public const string CRASH = "Chash";

        public GameObject CollisionObject { get; private set; }
        public LevelDescriptor LevelDescriptor { get; private set; }

        public DroneModel DroneModel { get; private set; }
        public WorldObjectType TypeBoost { get; private set; }
        public string DronId { get; private set; }
        public float SpeedBoost { get; private set; }
        public float AccelerationBoost { get; private set; }

        public WorldEvent(string name, GameObject target) : base(name, target)
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

        public WorldEvent(string name, float speedBoost, float accelerationBoost) : base(name)
        {
            SpeedBoost = speedBoost;
            AccelerationBoost = accelerationBoost;
        }

        public WorldEvent(string name, LevelDescriptor levelDescriptor, string dronId) : base(name)
        {
            LevelDescriptor = levelDescriptor;
            DronId = dronId;
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