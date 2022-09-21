using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Proviser2.Core.Model
{
    public class CourtClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }

        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public DateTime Date { get; set; }
        public string Judge { get; set; }

        [Indexed(Name = "ListingID", Order = 2, Unique = true)]
        public string Case { get; set; }
        public string Court { get; set; }
        public string Littigans { get; set; }
        public string Category { get; set; }
        public string Origin { get; set; }
        public DateTime SaveDate { get; set; }

        public override string ToString()
        {
            return $"{Date}\t{Judge}\t{Case}\t{Court}\t{Littigans}\t{Category}\t{Origin}";
        }

    }
}
