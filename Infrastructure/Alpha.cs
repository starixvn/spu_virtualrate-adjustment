namespace Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using DiffSharp.AD;
    using Gemynd.CSharp.Learning.Neural;
    using Model;

    [Serializable]
    public class Alpha
    {
        private readonly double learningRate;

        private readonly double errorThreshold;

        private readonly double windowedErrorThreshold;

        private double avgError;

        private bool stopper;

        private IEnumerable<MedoidsModel<TrainedModel>> models;

        public Alpha(double errorThreshold, double learningRate, double windowedErrorThreshold)
        {
            this.errorThreshold = errorThreshold;
            this.learningRate = learningRate;
            this.windowedErrorThreshold = windowedErrorThreshold;
        }

        public void Start()
        {
            stopper = false;
        }

        public void Stop()
        {
            stopper = true;
            Save(this, Constant.Alpha);
        }

        public void Initialize()
        {
            var raw = DataRepository.GetRawData();
            var clusters = DataRepository.ProcessedData(raw, windowedErrorThreshold).ToList();
            models = clusters.AsParallel().Select(Algorithm.GetMedoids).Select(Init).ToList();
            var prediction = clusters.AsParallel().SelectMany(Predict).ToList();
            Save(prediction, Constant.Prediction);
            avgError = prediction.Any() ? prediction.Average(p => p.TrainError) : 0;
            Save(this, Constant.Alpha);
        }

        private IEnumerable<Predicted> Predict(ClusterModel cModel)
        {
            var tmp =
                models.Where(m => m.HistoryLength == cModel.Size)
                    .SelectMany(m => m.Medoids)
                    .ToDictionary(m => m.CustId);

            var medoids = cModel.Series.Where(m => tmp.ContainsKey(m.CustId)).ToList();

            return cModel.Series.GroupBy(m => Min(m, medoids)).SelectMany(g => g.Select(m =>
            {
                var last = m.Data.Last();
                return new Predicted()
                {
                    CustId = m.CustId,
                    TrainError = Math.Abs(last[last.Length - 1] - tmp[g.Key].PredictMargin(last)),
                    Prediction = tmp[g.Key].PredictMargin(last.Skip(1).ToArray())
                };
            }));
        }

        private void Save(object obj, string name)
        {
            using (var file = File.Open(AppDomain.CurrentDomain.BaseDirectory + name, FileMode.OpenOrCreate))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(file, obj);
            }
        }

        private int Min(LearningModel model, IEnumerable<LearningModel> medoids)
        {
            var min = double.PositiveInfinity;
            LearningModel tmp = null;

            foreach (var marginCurve in medoids)
            {
                var d = Algorithm.Dtw(marginCurve.Curve, model.Curve);

                if (min > d)
                {
                    min = d;
                    tmp = marginCurve;
                }
            }

            return tmp.CustId;
        }

        private MedoidsModel<TrainedModel> Init(MedoidsModel<LearningModel> medoids)
        {
            var tsks = medoids.Medoids.Select(InitialTrain).ToArray();
            Task.WaitAll(tsks);
            return new MedoidsModel<TrainedModel>(medoids.HistoryLength, tsks.Select(tsk => tsk.Result).ToList());
        }

        private Task<TrainedModel> InitialTrain(LearningModel model)
        {
            var initWeight = Algorithm.GenerateWeight(Network.Complexity(model.Nn));
            return model.Train(initWeight, Float64.D.One * learningRate, errorThreshold, stopper);
        }

        private Task<TrainedModel> NewModel(TrainedModel trained, LearningModel model)
        {
            return model.Train(trained.TrainedWeights, Float64.D.One * learningRate, errorThreshold, stopper);
        }
    }
}