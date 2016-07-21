using System.Linq;

namespace Agent.Model
{
    public struct MarginCurve
    {
        public MarginCurve(IGrouping<int, BetModel> g)
        {
            Margins = g.OrderBy(m => m.Date).Select(m => m.Margin).ToArray();
            CustId = g.Key;
        }

        public int CustId { get; }

        public double[] Margins { get; }
    }
}