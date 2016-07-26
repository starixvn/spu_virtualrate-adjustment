using System;

namespace Agent.Model
{
    public struct BetModel
    {
        public int CustId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public double Winloss { get; set; }

        public double Turnover { get; set; }

        public double Margin => Winloss / Turnover;

        public DateTime Date => new DateTime(Year, Month, 1);
    }
}