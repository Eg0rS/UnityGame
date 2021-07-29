using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.SpeedBooster
{
    public class SpeedBoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SPEED_BUSTER;
        }
    }
}