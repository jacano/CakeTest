using System;
using System.Linq;
using System.Reflection;

namespace FakeBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: FakeBuilder [platform] [target]");
                return;
            }

            var platformName = args[0];
            var targetName = args[1];

            var platformType = PlatfomHelper.GetPlatformType(platformName);
            if (platformType == null)
            {
                Console.WriteLine($"{platformName} was not found.");
                return;
            }

            var currentPlatform = (PlatformBase)Activator.CreateInstance(platformType);
            currentPlatform.Run(targetName);

            Console.WriteLine();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Hit any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
