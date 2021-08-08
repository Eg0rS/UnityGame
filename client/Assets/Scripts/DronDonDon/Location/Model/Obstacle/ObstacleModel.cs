using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.Obstacle
{
    public class ObstacleModel : PrefabModel
    {
        public float Damage { get; private set; }
        public void Awake()
        {
            ObjectType = WorldObjectType.OBSTACLE;
            Damage = 3f;
        }
    }
}