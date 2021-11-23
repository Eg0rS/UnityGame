using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.Service.Game;
using Drone.PowerUp;
using Drone.PowerUp.Descriptor;
using Drone.PowerUp.Service;
using Drone.World;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Location.Service.Accelerator
{
    public class AcceleratorService : IWorldServiceInitiable
    {
        private const PowerUpType TYPE = PowerUpType.ACCELERATOR;
        private const string DURATION = "Duration";
        private const string ACCELERATION = "Acceleration";
        private const string ENERGY_COST = "EnergyCost";
        
        [Inject]
        private PowerUpService _powerUpService;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        [Inject]
        private IoCProvider<GameService> _gameService;

        private PowerUpDescriptor _powerUpDescriptor;
        private AcceleratorModel _acceleratorModel;

        public void Init()
        {
            InitParameters();

            _gameWorld.Require().AddListener<AcceleratorEvent>(AcceleratorEvent.PICKED, OnPowerUpPicked);
        }

        private void InitParameters()
        {
            _powerUpDescriptor = _powerUpService.GetDescriptorByType(TYPE);
            float duration = float.Parse(_powerUpDescriptor.GetParameterValue(DURATION));
            float acceleration = float.Parse(_powerUpDescriptor.GetParameterValue(DURATION));
            float energyCost = float.Parse(_powerUpDescriptor.GetParameterValue(DURATION));
            _acceleratorModel = new AcceleratorModel(duration, acceleration, energyCost);
        }

        private void OnPowerUpPicked(AcceleratorEvent acceleratorEvent)
        {
            _gameService.Require();
            _gameWorld.Require().Dispatch(new AcceleratorEvent(AcceleratorEvent.ACCELERATION, _acceleratorModel));
        }
    }
}