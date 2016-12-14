using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakeBuilder
{
    public static class BuildHelper
    {
        private const string NugetDownloadUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

        public static void InitCommon(PlatformBuild build)
        {
        }

        public static void UploadToTeamCity(PlatformBuild build)
        {
        }

        public static void GenerateArtifactsAndMove(PlatformBuild build)
        {
        }

        public static void Clear(PlatformBuild build)
        {
        }

        private static void DownloadNuget(PlatformBuild build)
        {
        }

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
    }
}