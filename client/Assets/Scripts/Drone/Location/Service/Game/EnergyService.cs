using System.Collections;
using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.World.Drone.Event;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service.Game
{
    public class EnergyService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float UPDATE_PERIOD = 1.0f;
        private const float BATTERY_ENERGY = 3.0f;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        private Coroutine _degreaseEnergy;
        private float _energyValue;
        private float _energyDecrement;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, OnSetParameters);
            _gameWorld.Require().AddListener<ControllEvent>(ControllEvent.START_GAME, OnStartGame);
            _gameWorld.Require().AddListener<EnergyEvent>(EnergyEvent.PICKED, OnTakeBattery);
        }

        private void OnTakeBattery(EnergyEvent energyEvent)
        {
            _energyValue += BATTERY_ENERGY;
        }

        private void OnStartGame(ControllEvent controllEvent)
        {
            _degreaseEnergy = StartCoroutine(DegraseEnergy());
        }

        private void OnSetParameters(WorldEvent worldEvent)
        {
            _energyValue = worldEvent.DroneModel.energy;
            _energyDecrement = worldEvent.DroneModel.energyFall;
            _gameWorld.Require().Dispatch(new EnergyEvent(EnergyEvent.ENERGY_UPDATE, _energyValue));
        }

        private IEnumerator DegraseEnergy()
        {
            while (true) {
                _energyValue -= _energyDecrement;
                _gameWorld.Require().Dispatch(new EnergyEvent(EnergyEvent.ENERGY_UPDATE, _energyValue));
                if (_energyValue > 0) {
                    yield return new WaitForSeconds(UPDATE_PERIOD);
                } else {
                    _gameWorld.Require().Dispatch(new EnergyEvent(EnergyEvent.ENERGY_FALL));
                    StopCoroutine(_degreaseEnergy);
                }
            }
        }
    }
}