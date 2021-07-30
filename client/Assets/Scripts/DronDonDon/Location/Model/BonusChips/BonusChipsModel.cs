using System.Collections;
using System.Collections.Generic;
using DronDonDon.Location.Model.BaseModel;
using UnityEngine;

namespace DronDonDon.Location.Model.BonusChips
{
    public class BonusChipsModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.BONUS_CHIPS;
        }
    }
}