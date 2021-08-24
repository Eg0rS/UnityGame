﻿using AgkCommons.Event;

namespace DeliveryRush.Billing.Event
{
    public class BillingEvent : GameEvent
    {
        public const string UPDATED = "resourceUpdated";

        public BillingEvent(string name) : base(name)
        {
        }
    }
}