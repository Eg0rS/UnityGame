namespace DronDonDon.Location.World.Dron
{
    public class Dron
    {
        private string _id;
        private int _speed;
        private int _durability;
        private int _mobility;

        public Dron(string id, int speed, int durability, int mobility)
        {
            _id = id;
            _speed = speed;
            _mobility = mobility;
            _durability = durability;
        }
    }
}