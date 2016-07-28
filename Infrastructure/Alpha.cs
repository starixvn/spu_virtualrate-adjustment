using System.Diagnostics;
using System.Xml.Linq;
using Agent.Criteria;
using Microsoft.FSharp.Collections;

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
        private readonly double _learningRate;

        private readonly double _errorThreshold;

        private readonly double _windowedErrorThreshold;

        private bool _stopper;

        private string name = nameof(Alpha);

        public Alpha(double errorThreshold, double learningRate, double windowedErrorThreshold)
        {
            this._errorThreshold = errorThreshold;
            this._learningRate = learningRate;
            this._windowedErrorThreshold = windowedErrorThreshold;
        }

        public void Initialize()
        {
            try
            {
                EventLog.WriteEntry(name, "start");
                var raw = DataRepository.GetRawData(name);

                EventLog.WriteEntry(name, "clustering");
                var clusters = DataRepository.ProcessedData(raw, _windowedErrorThreshold).ToList();

                EventLog.WriteEntry(name, "finish clustering,start training");
                var tsks = clusters.SelectMany(c => c.Series.Select(m => InitialTrain(m).ContinueWith(tsk => Predict(tsk.Result, m)))).ToArray();
                Task.WaitAll(tsks);

                EventLog.WriteEntry(name, "finish training, predicting");
                var tsks2 = SeqModule.ChunkBySize(1000, tsks.Select(tsk => tsk.Result)).Select(arr => Task.Run(() => InsertData(arr))).ToArray();
                Task.WaitAll(tsks2);

                EventLog.WriteEntry(name, "finish uploading data");
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(name, $"Error {ex.Message}");
            }
        }

        public void Stop()
        {
            _stopper = true;
        }

        private Prediction Predict(TrainedModel model, LearningModel model1)
        {
            var last = model1.Data.Last();
            var prediction = new Prediction()
            {
                CustId = model.CustId,
                Margin = model.PredictMargin(last.Skip(1).ToArray()),
                TrainError = Math.Abs(last[last.Length - 1] - model.PredictMargin(last))
            };

            return prediction;
        }

        private Task<TrainedModel> InitialTrain(LearningModel model)
        {
            var initWeight = Algorithm.GenerateWeight(Network.Complexity(model.Nn));
            return model.Train(initWeight, Float64.D.One * _learningRate, _errorThreshold, _stopper);
        }

        private void InsertData(Prediction[] arr)
        {
            var elems = arr.Select(p => p.ToXML()).ToArray();

            var criteria = new InsertDataCriteria()
            {
                Predictions = new XElement("ROOT", elems).ToString()
            };

            EventLog.WriteEntry(name, "uploading data");

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }
    }
}