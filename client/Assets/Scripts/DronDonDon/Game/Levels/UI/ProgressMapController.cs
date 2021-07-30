using System;
using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Core.Service;
using DronDonDon.Game.Levels.IoC;
using DronDonDon.Game.Levels.Model;
using DronDonDon.Game.Levels.Service;
using IoC.Attribute;
using UnityEngine;
using AgkUI.Core.Model;
using DronDonDon.Game.Levels.Event;
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

        [Inject]
        private LevelDescriptorRegistry _levelDescriptorRegistry;
        
        public static List<ProgressMapItemController> _progressMapItemController = new List<ProgressMapItemController>();

        private static List<GameObject> _selectedOutline = new List<GameObject>();
        
        
        [UICreated]
        public void Init()
        {
            _levelService.AddListener<LevelEvent>(LevelEvent.UPDATED, OnLevelMapUpdated);
                  _levelService.ResetPlayerProgress();
             _levelService.CreateLevelById("level2");
             _levelService.SetLevelProgress("level1", 1,1,0);
             PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
             playerProgressModel.CurrentLevel = "level2";
             _levelService.SetLevelProgress("level2", 1,1,1 );
            // //_levelService.DeletePlayerProgress();
            CreateSpots();
        }
        
        private void OnLevelMapUpdated(LevelEvent levelEvent)
        {
          //  CreateSpots();
            PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
            _levelViewModels = _levelService.GetLevels();
            // foreach (ProgressMapItemController item in _progressMapItemController)
            // {
            //     item.Init();
            // }

            for (int i = 0; i < 5; i++)
            {
                _progressMapItemController[i].Init(_levelViewModels[i], _levelViewModels[i].LevelDescriptor.Id == playerProgressModel.CurrentLevel);
            }
        }

        private void CreateSpots()
        {
            _levelViewModels = _levelService.GetLevels();
            PlayerProgressModel playerProgressModel = _levelService.RequireProgressModel();
            foreach (LevelViewModel item in _levelViewModels)
            {
                GameObject levelContainer = GameObject.Find($"level{item.LevelDescriptor.Order}");
                {
                    _uiService.Create<ProgressMapItemController>(UiModel
                            .Create<ProgressMapItemController>(item, item.LevelDescriptor.Id == playerProgressModel.CurrentLevel)
                            .Container(levelContainer))
                        .Done();
                }
            }
        }

        public static void AddSelectedLevel(GameObject item)
        {
            _selectedOutline.Add(item);
           
        }
        
        public static void UnEnableSelectedLevel()
        {
            foreach (GameObject item in _selectedOutline)
             {
                 item.SetActive(false);
             }
             _selectedOutline = new List<GameObject>();
        }
    }
}