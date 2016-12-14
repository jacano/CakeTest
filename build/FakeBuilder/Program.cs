using System;
using System.Linq;
using System.Reflection;

namespace FakeBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine($"Usage: FakeBuilder [platform] [target]");
                return;
            }

            var platformName = args[0];
            var targetName = args[1];

            var executingAssembly = Assembly.GetExecutingAssembly();
            var platform = BuildHelper.FindTypes(new[] {executingAssembly},
                type =>
                    string.Equals(type.Name, platformName, StringComparison.InvariantCultureIgnoreCase) &&
                    type.IsSubclassOf(typeof(PlatformBuild)))
                    .FirstOrDefault();

            if (platform == null)
            {
                Console.WriteLine($"{platformName} was not found.");
                return;
            }

            var currentPlatform = (PlatformBuild)Activator.CreateInstance(platform);
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
