using System.ServiceProcess;
using System.Threading.Tasks;
using Agent;
using Fanex.Data.Configuration;

namespace SPU.CustMarginPrediction.Service
{
    public partial class Service1 : ServiceBase
    {
        private Alpha agent;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DbSettingProviderManager.Current.Start();

            Task.Run(() =>
            {
                agent = new Alpha(Resource.ErrorThreshold, Resource.LearningRate, Resource.WindowThreshold);
                agent.Initialize();
            });
        }

        protected override void OnStop()
        {
            agent.Stop();
        }
    }
}