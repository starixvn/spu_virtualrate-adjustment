using System;
using Agent;

namespace AlphaCmd
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = new Alpha(0.012, 7, 0.3);
            a.Initialize();
            Console.ReadKey();
        }
    }
}