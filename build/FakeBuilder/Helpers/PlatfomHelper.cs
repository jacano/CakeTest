using FakeBuilder.Helpers;
using Microsoft.FSharp.Core;
using RunProcessAsTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeBuilder
{
    public static class PlatfomHelper
    {
        public static IEnumerable<Type> FindTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    Type[] exportedTypes = null;
                    try
                    {
                        exportedTypes = assembly.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        exportedTypes = e.Types;
                    }

                    if (exportedTypes != null)
                    {
                        foreach (var type in exportedTypes)
                        {
                            if (predicate(type))
                                yield return type;
                        }
                    }
                }
            }
        }

        public static Type GetPlatformType(string platformName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var platform = FindTypes(new[] { executingAssembly },
                type =>
                    string.Equals(type.Name, platformName, StringComparison.InvariantCultureIgnoreCase) &&
                    type.IsSubclassOf(typeof(PlatformBase)))
                    .FirstOrDefault();

            return platform;
        }

        public static PlatformBase GetPlatformInstance(Type type, IEnumerable<string> args)
        {
            var platformInstance = (PlatformBase)Activator.CreateInstance(type);
            platformInstance.Args = args.Skip(2);
            return platformInstance;
        }

        public static async Task<int> Execute(string command, string arguments)
        {
            Console.WriteLine($"{command} {arguments}");
            ProcessStartInfo p = new ProcessStartInfo(command)
            {
                Arguments = arguments,
                RedirectStandardOutput = true,
            };

            var process = await ProcessEx.RunAsync(p);
            var output = process.StandardOutput;
            Console.WriteLine(string.Join("\n", output));
            return process.ExitCode;
        }
    }
}