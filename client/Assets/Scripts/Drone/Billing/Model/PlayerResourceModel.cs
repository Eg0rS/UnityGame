namespace Drone.Billing.Model
{
    public class PlayerResourceModel
    {
        private int _credits;
        private int _crypto;
        public int CreditsCount
        {
            get { return _credits; }
            set { _credits = value; }
        }
        public int CryptoCount
        {
            get { return _crypto; }
            set { _crypto = value; }
        }
    }
}