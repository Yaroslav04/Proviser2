using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class DecisionSoketClass : DecisionClass
    {
        public string DecisionDateSoket { get; set; }
        public string LegalDateSoket { get; set; }
        public string CategorySoket { get; set; }
        public string Header { get; set; }
        public DecisionSoketClass(DecisionClass decisionClass)
        {
            this.N = decisionClass.N;
            this.Id = decisionClass.Id;
            this.DecisionType = decisionClass.DecisionType;
            this.DecisionDate = decisionClass.DecisionDate;
            this.LegalDate = decisionClass.LegalDate;
            this.JudiciaryType = decisionClass.JudiciaryType;
            this.Case = decisionClass.Case;
            this.Court = decisionClass.Court;
            this.Judge = decisionClass.Judge;
            this.URL = decisionClass.URL;
            this.Content = decisionClass.Content;
            this.CriminalNumber = decisionClass.CriminalNumber;
            this.Category = decisionClass.Category;
            this.SaveDate = decisionClass.SaveDate;
        }
    }
}
