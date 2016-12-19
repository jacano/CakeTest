using FakeBuilder.Helpers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var platform = PlatfomHelper.FindTypes(new[] { executingAssembly },
                type =>
                    string.Equals(type.Name, platformName, StringComparison.InvariantCultureIgnoreCase) &&
                    type.IsSubclassOf(typeof(PlatformBase)))
                    .FirstOrDefault();
            return platform;
        }
    }
}