using System;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;


namespace DronDonDon.Location.UI
{
    [UIController("UI/Dialog/pfGameOverlay@embeded")]
    public class DronStatsDialog :MonoBehaviour
    {
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [UIComponentBinding("CountChips")] 
            private UILabel _countChips;

        [UIComponentBinding("ParceTime")] 
            private UILabel _timer;
            
        [UIComponentBinding("CountEnergy")] 
            private UILabel _countEnergy;
            
                
        [UIComponentBinding("CountDurability")] 
            private UILabel _durability;

        private float _time=0;

        private bool _isGame=false;

        private float _MaxDurability=0;
        
        [UICreated]
        private void Init(DronStats dronStats)
        {
            _MaxDurability = dronStats._durability;    //для вывода в процентах
            SetStats(dronStats);
            _timer.text = "0,00";
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.UI_UPDATE, UiUpdate);
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.START_GAME, StartGame);
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.END_GAME, EndGame);
        }

        private void EndGame(WorldObjectEvent objectEvent)
        {
            _isGame = false;
        }

        private void StartGame(WorldObjectEvent objectEvent)
        {
            _isGame = true;
        }

        private void Update()
        {
            if (_isGame)
            {
                _time += Time.deltaTime;
                _timer.text = _time.ToString("F2");
            }
        }

        private void UiUpdate(WorldObjectEvent objectEvent)
        {
         SetStats(objectEvent._dronStats);   
        }

        private void SetStats(DronStats dronStats)
        {
            _countChips.text = dronStats._countChips.ToString();
            _countEnergy.text = dronStats._energy.ToString("F0");
            _durability.text = ((dronStats._durability / _MaxDurability) * 100).ToString("F0") + "%";
            
        }
        
        [UIOnClick("PauseButton")]
        private void OnPauseButton()
        {
            _dialogManager.Require().ShowModal<LevelPauseDialog>();
        }
        
        
    }
}