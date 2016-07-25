using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using Agent;
using Fanex.Data.Configuration;

namespace AlphaCmd
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DbSettingProviderManager.Current.Start();
            var a = new Alpha(0.012,7,0.3);
            a.Initialize();
        }
    }
}