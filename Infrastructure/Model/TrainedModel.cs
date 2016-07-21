using System;
using DiffSharp.AD;

namespace Agent.Model
{
    [Serializable]
    public class TrainedModel
    {
        private readonly Func<Float64.DV, double[], Float64.D> _predictor;

        public TrainedModel(int custId, Float64.DV weights, Func<Float64.DV, double[], Float64.D> predictor)
        {
            CustId = custId;
            TrainedWeights = weights;
            this._predictor = predictor;
        }

        public int CustId { get; }

        public Float64.DV TrainedWeights { get; }

        public double PredictMargin(double[] recentHistory)
        {
            return (double)_predictor(TrainedWeights, recentHistory);
        }
    }
}