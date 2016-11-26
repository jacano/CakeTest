using CodeCake;
using System;

namespace CakeBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CodeCakeApplication(Environment.CurrentDirectory);
            var result = app.Run(args);
            Console.WriteLine();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Hit any key to exit.");
                Console.ReadKey();
            }
            return result;
        }
    }
}
