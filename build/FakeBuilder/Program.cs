using System;

namespace FakeBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new AndroidBuild();

            Console.WriteLine();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Hit any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
