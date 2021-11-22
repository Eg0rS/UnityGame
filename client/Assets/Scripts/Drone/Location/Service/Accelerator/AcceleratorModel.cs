namespace Drone.Location.Service.Accelerator
{
    public class AcceleratorModel
    {
        private float _accelerationForce;
        private float _accelerationDuration;
        private float _energyCost;
        public float AccelerationDuration
        {
            get { return _accelerationDuration; }
            private set { _accelerationDuration = value; }
        }
        public float AccelerationForce
        {
            get { return _accelerationForce; }
            private set { _accelerationForce = value; }
        }

        public float EnergyCost
        {
            get { return _energyCost; }
            private set { _energyCost = value; }
        }

        public AcceleratorModel(float force, float duration, float energyCost)
        {
            _accelerationForce = force;
            _accelerationDuration = duration;
            _energyCost = energyCost;
        }
    }
}