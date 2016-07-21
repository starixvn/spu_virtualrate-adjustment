using Fanex.Data.Repository;

namespace Agent.Criteria
{
    public class GetDataCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "CMS_GetRawData";
        }
    }
}