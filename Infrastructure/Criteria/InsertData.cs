using Fanex.Data.Repository;

namespace Agent.Criteria
{
    internal class InsertDataCriteria : CriteriaBase
    {
        public string Predictions { get; set; }

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