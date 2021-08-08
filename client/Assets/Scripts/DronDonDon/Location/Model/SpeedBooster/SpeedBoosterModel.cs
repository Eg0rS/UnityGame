using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.SpeedBooster
{
    public class SpeedBoosterModel : PrefabModel
    {
        public float SpeedBoost { get; private set; }
        public float Duration { get; private set; }

        public void Awake()
        {
            ObjectType = WorldObjectType.SPEED_BUSTER;
            SpeedBoost = 45f;
            Duration = 5f;
        }
    }
}