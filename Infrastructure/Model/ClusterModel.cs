using System.Collections.Generic;
using System.Linq;

namespace Agent.Model
{
    public class ClusterModel
    {
        public ClusterModel(IGrouping<int, LearningModel> g)
        {
            Size = g.Key;
            Series = g.AsEnumerable().ToList();
        }

        public int Size { get; }

        public IList<LearningModel> Series { get; }
    }
}