using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class LogClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Teg { get; set; }
        public string Value { get; set; }
        public string Case { get; set; }
        public bool Result { get; set; }

    }
}
