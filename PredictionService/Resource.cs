using System.Configuration;

namespace SPU.CustMarginPrediction.Service
{
    public static class Resource
    {
        public static double LearningRate
        {
            get { return double.Parse(ConfigurationManager.AppSettings["learning rate"]); }
        }

        public static double ErrorThreshold
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings["error threshold"]);
            }
        }

        public static double WindowThreshold
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings["windowed threshold"]);
            }
        }
    }
}