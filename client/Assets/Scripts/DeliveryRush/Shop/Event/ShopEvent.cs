﻿using AgkCommons.Event;

namespace DeliveryRush.Shop.Event
{
    public class ShopEvent : GameEvent
    {
        public const string CLOSE_DIALOG = "closeDialog";

        public ShopEvent(string name) : base(name)
        {
        }
    }
}