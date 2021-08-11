using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Element.Text;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Location.UI
{
    [UIController("UI/Dialog/pfDronState@embeded")]
    public class DronStatsDialog : GameEventDispatcher
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        [UIComponentBinding("CountChipsText")]
        private UILabel _countChips;
        
        [UIComponentBinding("TimerText")]
        private UILabel _timer;
        
        [UIComponentBinding("DurabilityText")]
        private UILabel _durability;
        
        [UIComponentBinding("EnergyText")]
        private UILabel _energy;

        [UICreated]
        public void Init(DronStats dronStats)
        {
            UpdateStats(dronStats);
        }
        
        private void Start()
        {
            _gameWorld.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.UI_UPDATE, UiUpdate); ;
        }
        
        private void UiUpdate(WorldObjectEvent objectEvent)
        {
            DronStats dronStats = objectEvent._dronStats;
            UpdateStats(dronStats);
        }

        private void UpdateStats(DronStats dronStats)
        {
            if (dronStats._durability <= 0)
            {
                _durability.text = "0";
            }
            else
            {
                _durability.text = dronStats._durability.ToString();
            }

            _countChips.text = dronStats._countChips.ToString();
            _energy.text = dronStats._energy.ToString();
        }

        [UIOnClick("StopButton")]
        private void CloseButton()
        {
       
        }
        
        [UIOnClick("ShieldButton")]
        private void ShieldBoost()
        {
          
        }
        
        [UIOnClick("SpeedButton")]
        private void SpeedBoost()
        {
            
        }
    }
}