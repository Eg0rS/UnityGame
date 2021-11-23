﻿using AgkCommons.Event;
using Drone.Descriptor;
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
        
        public const string END_GAME = "EndGame";
        public const string FINISHED = "Finished";
        public const string WORLD_CREATED = "WorldCreated";
        public const string SET_DRON_PARAMETERS = "SetDronParameters";
        public const string DRONE_LETHAL_CRASH = "DroneLethalCrash";
        public const string ENABLE_SHIELD = "EnableShield";
        public const string DISABLE_SHIELD = "DisableShield";
        public const string TAKE_CHIP = "TakeChip";
        public const string TAKE_SHIELD = "TakeShield";
        public const string TAKE_X2 = "TakeX2";
        public const string TAKE_MAGNET = "TakeMagnet";

        public DroneModel DroneModel { get; private set; }


        public GameObject Drone { get; private set; }

        public WorldEvent(string name, GameObject target) : base(name, target)
        {
            Drone = target;
        }

        public WorldEvent(string name, DroneModel droneModel) : base(name)
        {
            DroneModel = droneModel;
        }
        
        

        public WorldEvent(string name) : base(name)
        {
        }
    }
}