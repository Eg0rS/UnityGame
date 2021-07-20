﻿using AgkCommons.Event;

namespace DronDonDon.Billing.Event
{
    public class BillingEvent  : GameEvent
    {
        public const string UPDATED = "moneyUpdated";
        
        public BillingEvent(string name) : base(name)
        {
        }
    }
}