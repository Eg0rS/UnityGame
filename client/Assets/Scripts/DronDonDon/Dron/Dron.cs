namespace DronDonDon.Dron
{
    public class Dron
    {
        public Dron(int energy, int durability)
        {
            // _initialEnergy = _currentEnegry = energy;
            // _initialDurability = _currentDurability = strength;
            _currentEnergy = energy;
            _durability = durability;
            _ChipsCollected = 0;
            
            // Положение дрона по умолчанию — центральная ячейка (0,0)
            _containerCellPositionX = 0;
            _containerCellPositionY = 0;
        }

        private sbyte _containerCellPositionX;
        private sbyte _containerCellPositionY;

        // private int _initialEnergy;
        private int _currentEnergy;
        public int Energy
        {
            get => _currentEnergy;
            set => _currentEnergy += value;
        }

        // private int _initialDurability;
        private int _durability;
        public int Durability
        {
            get => _durability;
            set => _durability += value;
        }

        private int _ChipsCollected;
        public int Chips
        {
            get => _ChipsCollected;
            set => _ChipsCollected += value;
        }
    }
}