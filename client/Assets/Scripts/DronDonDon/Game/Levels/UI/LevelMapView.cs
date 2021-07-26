using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using NUnit.Framework;
using UnityEngine;

namespace DronDonDon.Game.Levels.UI
{
    [UIController(_prefabNameController)]
    public class LevelMapView
    {
        private const string _prefabNameController = "name";
        
        [Inject] 
        private LevelService _levelService;

        private List<LevelViewModel> _levelViewModels;

        private List<GameObject> _levelContainers;

        [UICreated]
        private void Init()
        {
            _levelViewModels = _levelService.GetLevels();
            _levelContainers = FindLevelContainers();
        }

        private List<GameObject> FindLevelContainers()
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = 1; i < _levelViewModels.Count; i++)
            {
                GameObject temp = GameObject.Find($"missionContainer{i}");
                result.Add(temp);
            }

            return result;
        }
    }
}