using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Proviser2.Core.Model
{
    public class JournalClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        public int HearingId { get; set; }
        public string Type { get; set; }
        public string Event { get; set; }
    }

}
