﻿using UnityEngine;

namespace Drone.Location.Model.BaseModel
{
    public class PrefabModel : MonoBehaviour
    {
        private WorldObjectType _objectType;
        public WorldObjectType ObjectType
        {
            get { return _objectType; }
            protected set { _objectType = value; }
        }
    }
}