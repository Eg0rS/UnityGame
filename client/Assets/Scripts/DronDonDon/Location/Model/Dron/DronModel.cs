using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.Dron
{
    public class DronModel : PrefabModel
    {
        public float SpeedShift = 2;
        
        
        
        public void Awake()
        {
            ObjectType = WorldObjectType.DRON;
        }
    }
}