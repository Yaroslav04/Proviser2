using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class CourtSoketClass : CourtClass
    {
        public string PrisonDate { get; set; }
        public string Header { get; set; }
        public string Note { get; set; }
        public CourtSoketClass(CourtClass courtClass)
        {
            this.Date = courtClass.Date;
            this.Judge = courtClass.Judge;
            this.Case = courtClass.Case;
            this.Court = courtClass.Court;
            this.Littigans = courtClass.Littigans;
            this.Category = courtClass.Category;
            this.Origin = courtClass.Origin;
        }
    }
}
