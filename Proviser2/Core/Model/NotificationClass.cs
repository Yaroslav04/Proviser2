using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class NotificationClass
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Link{ get; set; }
        public string Court { get; set; }


    }
}
