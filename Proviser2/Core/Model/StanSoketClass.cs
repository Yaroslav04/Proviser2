using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Model
{
    public class StanSoketClass : StanClass
    {
        public string Header { get; set; }

        public StanSoketClass(StanClass stanClass)
        {
          this.N = stanClass.N;
          this.Court = stanClass.Court;
          this.Case = stanClass.Case;
          this.SubCase = stanClass.SubCase;
          this.RegDate = stanClass.RegDate;
          this.Judge = stanClass.Judge;
          this.SubJudge = stanClass.SubJudge;
          this.Littigans = stanClass.Littigans;
          this.Date = stanClass.Date;
          this.Decision = stanClass.Decision;
          this.SubDecision = stanClass.SubDecision;
          this.Category = stanClass.Category;
          this.Description = stanClass.Description;
          this.SaveDate = stanClass.SaveDate;
    }
    }
}
