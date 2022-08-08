using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SQLite;

namespace Proviser2.Core.Model
{
    public class CaseClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }

        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Case { get; set; }
        public string Header { get; set; }
        public string Note { get; set; }
        public DateTime PrisonDate { get; set; }
        public string CriminalNumber{ get; set; }
        public string MainCase{ get; set; }
        public string Status { get; set; }

    }
}
