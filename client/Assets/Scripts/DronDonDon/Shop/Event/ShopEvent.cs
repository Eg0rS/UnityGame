using AgkCommons.Event;

namespace DronDonDon.Shop.Event
{
    public class ShopEvent : GameEvent
    {
        public const string UPDATED = "levelUpdated";
        public ShopEvent(string name) : base(name)
        {
        }  
    }
}