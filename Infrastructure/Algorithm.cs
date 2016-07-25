namespace Agent
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DiffSharp.AD;
    using Gemynd.CSharp.Learning.InstanceBase;
    using Gemynd.CSharp.Sequences;
    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;
    using Model;
    using Network = Gemynd.CSharp.Learning.Neural.Network;

    public static class Algorithm
    {
        public static Func<double[], double[], double> Dtw { get; } = Metric.DTW((double x, double y) => Math.Pow(x - y, 2));

        public static MedoidsModel<LearningModel> GetMedoids(ClusterModel model)
        {
            var clusterNum = model.Series.Count / 10;
            var medoids =
                Clustering.KMedoids(model.Series, (x, y) => Math.Sqrt(Dtw(x.Curve, y.Curve)), clusterNum)
                .Select(i => model.Series[i]);
            return new MedoidsModel<LearningModel>(model.Size, medoids.ToList());
        }

        public static Task<TrainedModel> Train(this LearningModel model, Float64.DV weights, Float64.D learningRate, double errorThreshold, bool stopper)
        {
            var network = model.Nn;

            var func = Network.Compile(network).Head;
            Func<Float64.DV, double[], Float64.D> cost = (ws, e) => Float64.D.Pow(func(ws, e) - e[model.AttributeLength], 2) / 2;

            var parameters = Gemynd.CSharp.Learning.Neural.Training.Parameter(weights, learningRate);

            var trainer = Gemynd.CSharp.Learning.Neural.Training.Trainer(
                cost,
                (err1, err, v) => (double)err1 < errorThreshold || stopper,
                FSharpOption<Action<Float64.DV, Float64.D>>.None);

            return
                FSharpAsync
                .StartAsTask(
                    Gemynd.CSharp.Learning.Neural.Training.LevenbergMarquadt(model.Data, parameters, trainer),
                    FSharpOption<TaskCreationOptions>.None,
                    FSharpOption<CancellationToken>.None)
                .ContinueWith(tsk => new TrainedModel(model.CustId, tsk.Result, func));
        }

        public static Float64.DV GenerateWeight(int length)
        {
            var r = new Random();
            var arr = new Float64.D[length];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = r.Next(2) == 1 ? Float64.D.One * r.NextDouble() : -Float64.D.One * r.NextDouble();
            }

            return Float64.DV.OfArray(arr);
        }
    }
}