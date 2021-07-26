using System;
using DronDonDon.Core.Repository;
using DronDonDon.Game.Levels.Model;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

namespace DronDonDon.Game.Levels
{
    public class ProgressRepository  : LocalPrefsSingleRepository<PlayerProgressModel> //LocalPrefsSingleRepository<LevelViewModel>
    {
        public ProgressRepository() : base("progressRepository")    
        {
            
        }
    }
}