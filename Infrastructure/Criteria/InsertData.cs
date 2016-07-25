using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace Agent.Criteria
{
    class InsertDataCriteria:CriteriaBase
    {
        public int CustId { get; set; }
        public double TrainErr { get; set; }
        public double Prediction { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "CMS_InsertPrediction";
        }
    }
}
