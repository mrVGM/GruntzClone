﻿using Base.MessagesSystem;
using Base.Status;

namespace Gruntz.Statuses
{
    public class OverrideActorControllerStatusDef : StatusDef
    {
        public MessagesBoxTagDef MessagesBoxTagDef;
        protected override StatusData StatusData
        {
            get
            {
                return new OverrideActorControllerStatusData();
            }
        }
    }
}
