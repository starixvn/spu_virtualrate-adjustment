using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Model
{
    [Serializable]
    public class Predicted
    {
        public int CustId { get; set; }

        public double TrainError { get; set; }

        public double Prediction { get; set; }
    }
}
