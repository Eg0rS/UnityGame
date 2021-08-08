using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.Finish
{
    public class FinishModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.FINISH;
        }
    }
}