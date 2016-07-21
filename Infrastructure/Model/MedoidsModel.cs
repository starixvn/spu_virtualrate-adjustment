namespace Agent.Model
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public struct MedoidsModel<T>
    {
        public MedoidsModel(int length, IEnumerable<T> medoids)
        {
            HistoryLength = length;
            Medoids = medoids;
        }

        public int HistoryLength { get; }

        public IEnumerable<T> Medoids { get; }
    }
}