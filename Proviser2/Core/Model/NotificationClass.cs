using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class NotificationClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Type { get; set; }
        [Indexed(Name = "ListingID", Order = 2, Unique = true)]
        public string Description { get; set; }
        [Indexed(Name = "ListingID", Order = 3, Unique = true)]
        public string Case { get; set; }
        public string Link { get; set; }
        public DateTime SaveDate { get; set; }
        [Indexed(Name = "ListingID", Order = 4, Unique = true)]
        public int Day { get; set; }
        public bool IsNotificate { get; set; }
        public bool IsExecute { get; set; }

    }
}
