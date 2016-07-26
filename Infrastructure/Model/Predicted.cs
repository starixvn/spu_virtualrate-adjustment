using System;
using System.Xml.Linq;

namespace Agent.Model
{
    [Serializable]
    public struct Prediction
    {
        public int CustId { get; set; }

        public double TrainError { get; set; }

        public double Margin { get; set; }

        public XElement ToXML()
        {
            return new XElement("Prediction", new XAttribute("CustId", CustId), new XAttribute("TrainErr", TrainError), new XAttribute("Margin", Margin));
        }
    }
}