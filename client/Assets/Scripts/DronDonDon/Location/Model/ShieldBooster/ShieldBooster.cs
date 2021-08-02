using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.ShieldBooster
{
    public class ShieldBoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SHIELD_BUSTER;
        }
    }
}