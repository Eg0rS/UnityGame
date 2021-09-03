namespace Drone.Billing.Model
{
    public class PlayerResourceModel
    {
        private int _credits;

        public int CreditsCount
        {
            get { return _credits; }
            set { _credits = value; }
        }
    }
}