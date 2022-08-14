using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class EventSoketClass : EventClass
    {
        public EventSoketClass(EventClass eventClass)
        {
            this.N = eventClass.N;
            this.Case = eventClass.Case;
            this.Date = eventClass.Date;
            this.Event = eventClass.Event;
            this.Description = eventClass.Description;
        }

        public string DateSoket { get; set; }
        public string Header { get; set; } 
    }
}
