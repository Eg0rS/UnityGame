﻿using Drone.Location.Model;
using Drone.Location.Model.Battery;
using UnityEngine;

namespace Drone.Location.World.Battery
{
    public class BatteryController : MonoBehaviour, IWorldObjectController<BatteryModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(BatteryModel model)
        {
            ObjectType = model.ObjectType;
        }
    }
}