namespace Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Fanex.Data.Repository;
    using Model;

    public static class DataRepository
    {
        public static IEnumerable<BetModel> GetRawData()
        {
            var today = DateTime.Today;
            var q = new Queue<IEnumerable<BetModel>>();
            var rep = new DynamicRepository();

            for (var accDate = new DateTime(today.Year - 2, 1, 1); accDate.Month != today.Month && accDate.Year != today.Year; accDate = accDate.AddMonths(1))
            {
                var criteria = new GetDataCriteria()
                {
                    Month = accDate.Month,
                    Year = accDate.Year
                };

                q.Enqueue(rep.Fetch<BetModel>(criteria));
            }

            return q.SelectMany(x => x).ToList();
        }

        public static IEnumerable<ClusterModel> ProcessedData(IEnumerable<BetModel> raw, double errorThreshold)
        {
            return
                raw.GroupBy(m => m.CustId)
                    .Select(g => new LearningModel(new MarginCurve(g), errorThreshold))
                    .Where(m => m.Data.Any())
                    .GroupBy(s => s.Curve.Length)
                    .Select(g => new ClusterModel(g));
        }
    }
}