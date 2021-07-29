﻿using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Service;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using UnityEngine;
using AgkUI.Core.Model;
using NUnit.Framework;

namespace DronDonDon.Game.Levels.UI
{
    [UIController("UI/Panel/pfMiddlePanel@embeded")]
    public class ProgressMapController : MonoBehaviour
    {
        [Inject]
        private LevelService _levelService;
        
        [Inject] 
        private UIService _uiService;
        
        private List<LevelViewModel> _levelViewModels;

        private static List<GameObject> _itemControllers = new List<GameObject>();

        [UICreated]
        public void Init()
        {
            CreateSpots();
        }
        
        private void CreateSpots()
        {
            _levelViewModels = _levelService.GetLevels();
            List <LevelDescriptor> descriptors = _levelService.GetListLevelsDescriptors();
            for (int i = 0; i < descriptors.Count; i++)
            {
                GameObject levelContainer = GameObject.Find($"level{i+1}");
                if (_levelViewModels.Count > i)
                {
                    _uiService.Create<ProgressMapItemController>(UiModel
                            .Create<ProgressMapItemController>(_levelViewModels[i].LevelProgress,descriptors[i],false)
                            .Container(levelContainer))
                            .Done();
                }
                else if (_levelViewModels.Count == i)
                {
                    _uiService.Create<ProgressMapItemController>(UiModel
                            .Create<ProgressMapItemController>(new LevelProgress(),descriptors[i],true)
                            .Container(levelContainer))
                            .Done();
                }
                else
                {
                    _uiService.Create<ProgressMapItemController>(UiModel
                            .Create<ProgressMapItemController>(new LevelProgress(),descriptors[i],false)
                            .Container(levelContainer))
                            .Done();
                }
            }
        }

        public static void AddSelectedLevel(GameObject item)
        {
            _itemControllers.Add(item);
        }
        
        public static void UnEnableSelectedLevel(GameObject select)
        {
            foreach (GameObject item in _itemControllers)
            {
                item.SetActive(false);
            }
            _itemControllers = new List<GameObject>();
        }
    }
}