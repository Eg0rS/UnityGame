using Drone.Core.Service;
using Drone.Location.Event;
using Drone.PowerUp;
using Drone.PowerUp.Descriptor;
using Drone.PowerUp.Service;
using Drone.World;
using IoC.Attribute;

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
        private GameWorld _gameWorld;

        private PowerUpDescriptor _powerUpDescriptor;
        private AcceleratorModel _acceleratorModel;

        public void Init()
        {
            InitParameters();

            _gameWorld.AddListener<AcceleratorEvent>(AcceleratorEvent.PICKED, OnAcceleratorUpPicked);
        }

        private void InitParameters()
        {
            _powerUpDescriptor = _powerUpService.GetDescriptorByType(TYPE);
            float duration = float.Parse(_powerUpDescriptor.GetParameterValue(DURATION));
            float acceleration = float.Parse(_powerUpDescriptor.GetParameterValue(ACCELERATION));
            float energyCost = float.Parse(_powerUpDescriptor.GetParameterValue(ENERGY_COST));
            _acceleratorModel = new AcceleratorModel(duration, acceleration, energyCost);
        }

        private void OnAcceleratorUpPicked(AcceleratorEvent acceleratorEvent)
        {
            _gameWorld.Dispatch(new AcceleratorEvent(AcceleratorEvent.ACCELERATION, _acceleratorModel));
        }
    }
}