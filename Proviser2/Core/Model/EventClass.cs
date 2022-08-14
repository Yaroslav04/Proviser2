using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Proviser2.Core.Model
{
    public class EventClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        public string Case { get; set; }
        public DateTime Date { get; set; }
        public string Event { get; set; }
        public string Description { get; set; }
    }

}
