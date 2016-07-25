using Agent.Criteria;

namespace Agent
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DiffSharp.AD;
    using Fanex.Data;
    using Gemynd.CSharp.Learning.Neural;
    using Model;

    [Serializable]
    public class Alpha
    {
        private readonly double learningRate;

        private readonly double errorThreshold;

        private readonly double windowedErrorThreshold;

        private bool stopper;

        public Alpha(double errorThreshold, double learningRate, double windowedErrorThreshold)
        {
            this.errorThreshold = errorThreshold;
            this.learningRate = learningRate;
            this.windowedErrorThreshold = windowedErrorThreshold;
        }

        public void Initialize()
        {
            //Debugger.Launch();
            var raw = DataRepository.GetRawData();
            var clusters = DataRepository.ProcessedData(raw, windowedErrorThreshold).ToList();
            var tsks = clusters.SelectMany(c => c.Series.Select(m => InitialTrain(m).ContinueWith(tsk => Predict(tsk.Result, m)))).ToArray();
            Task.WaitAll(tsks);
        }

        public void Stop()
        {
            stopper = true;
        }

        private void Predict(TrainedModel model, LearningModel model1)
        {
            var last = model1.Data.Last();
            var criteria = new InsertDataCriteria()
            {
                CustId = model.CustId,
                Prediction = model.PredictMargin(last.Skip(1).ToArray()),
                TrainErr = Math.Abs(last[last.Length - 1] - model.PredictMargin(last))
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        private Task<TrainedModel> InitialTrain(LearningModel model)
        {
            var initWeight = Algorithm.GenerateWeight(Network.Complexity(model.Nn));
            return model.Train(initWeight, Float64.D.One * learningRate, errorThreshold, stopper);
        }
    }
}