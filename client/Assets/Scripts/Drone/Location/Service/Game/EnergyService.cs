using System.Collections;
using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.Service.Game.Event;
using Drone.World;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.Service.Game
{
    public class EnergyService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float UPDATE_PERIOD = 1.0f;

        [Inject]
        private GameWorld _gameWorld;
        private Coroutine _degreaseEnergy;
        private float _energyValue;
        private float _energyDecrement;

        public void Init()
        {
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);
            _gameWorld.AddListener<InGameEvent>(InGameEvent.START_GAME, OnStartGame);
            _gameWorld.AddListener<EnergyEvent>(EnergyEvent.CHANGED, OnChangeEnergy);
        }

        private void OnChangeEnergy(EnergyEvent energyEvent)
        {
            _energyValue += energyEvent.EnergyDelta;
        }

        private void OnStartGame(InGameEvent inGameEvent)
        {
            _degreaseEnergy = StartCoroutine(DecreaseEnergy());
        }

        private void OnSetParameters(InGameEvent inGameEvent)
        {
            _energyValue = inGameEvent.DroneModel.energy;
            _energyDecrement = inGameEvent.DroneModel.energyFall;
            _gameWorld.Dispatch(new EnergyEvent(EnergyEvent.UPDATE, _energyValue));
        }

        private IEnumerator DecreaseEnergy()
        {
            while (true) {
                _energyValue -= _energyDecrement;
                _gameWorld.Dispatch(new EnergyEvent(EnergyEvent.UPDATE, _energyValue));
                if (_energyValue > 0) {
                    yield return new WaitForSeconds(UPDATE_PERIOD);
                } else {
                    //death
                    StopCoroutine(_degreaseEnergy);
                }
            }
        }
    }
}