using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Proviser2.Core.Model
{
    public class ConfigClass
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

    }
}
