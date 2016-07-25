namespace Agent.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DiffSharp.AD;
    using Gemynd.CSharp.Sequences;
    using Gemynd.Learning.Neural;
    using Microsoft.FSharp.Collections;
    using Network = Gemynd.CSharp.Learning.Neural.Network;

    public class LearningModel
    {
        public LearningModel(MarginCurve s, double threshold)
        {
            CustId = s.CustId;

            var stoppingThreshold = 20; //stopping threshold suggested in Time Series Prediction and Neural Networks paper
            var tmp = Seq.FNN(s.Margins, stoppingThreshold, (x, y) => Math.Sqrt(x.Zip(y, (a, b) => Math.Pow(a - b, 2)).Sum())); // find good windows using false nearest neighbor

            Data =
                (double)tmp.Item2 / (s.Margins.Length - tmp.Item1 + 1) <= threshold // (number of error data)/(total data) <= threshold
                ? SeqModule.Windowed(tmp.Item1 + 1, s.Margins).ToArray()
                : new double[][] { };
            AttributeLength = tmp.Item1;
            Curve = s.Margins;
        }

        public int CustId { get; }

        public IEnumerable<double[]> Data { get; }

        public double[] Curve { get; }

        public int AttributeLength { get; }

        public Network<double[]> Nn
        {
            get
            {
                var n = AttributeLength;
                var inputs = new Neuron<double[]>[n];

                for (var i = 0; i < inputs.Length; i++)
                {
                    var tmp = i;
                    inputs[i] = Network.Input<double[]>(arr => arr[tmp]);
                }

                var hiddens = new Neuron<double[]>[n * 7];

                for (var i = 0; i < hiddens.Length; i++)
                {
                    hiddens[i] = Network.Hidden(0, true, Float64.D.Tanh, inputs);
                }

                var output = Network.Output(true, x => x, hiddens);
                var network = Network.Build(new[] { output });
                return network;
            }
        }
    }
}