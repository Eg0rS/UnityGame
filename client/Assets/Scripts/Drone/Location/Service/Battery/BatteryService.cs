using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.World;
using IoC.Attribute;

namespace Drone.Location.Battery.Service
{
    public class BatteryService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float BATTERY_ENERGY = 3.0f;
        [Inject]
        private GameWorld _gameWorld;

        public void Init()
        {
            _gameWorld.AddListener<EnergyEvent>(EnergyEvent.PICKED, OnTakeBattery);
        }

        private void OnTakeBattery(EnergyEvent energyEvent)
        {
            _gameWorld.Dispatch(new EnergyEvent(EnergyEvent.CHANGED, BATTERY_ENERGY));
        }
    }
}