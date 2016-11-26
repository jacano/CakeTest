using Cake.Common;
using Cake.Common.Build.TeamCity;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Net;
using Cake.Common.Tools.NuGet;
using System;
using System.Linq;

namespace CakeBuilder
{
    public static class BuildHelper
    {
        private const string NugetDownloadUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

        public static void InitCommon(PlatformBuild build)
        {
            build.buildNumber = build.Cake.EnvironmentVariable("BUILDNUMBER") ?? "1";

            build.artifactDir = build.Cake.Directory("artifacts");
            build.outputDir = build.Cake.Directory("output");
            build.toolsDir = build.Cake.Directory("tools");

            build.configuration = build.Cake.Argument("Configuration", "Debug");
        }

        public static void UploadToTeamCity(PlatformBuild build)
        {
            var teamCityProvider = new TeamCityProvider(build.Cake.Environment, build.Cake.Log);
            teamCityProvider.PublishArtifacts(build.artifactDir.Path.FullPath);
        }

        public static void GenerateArtifactsAndMove(PlatformBuild build)
        {
            foreach (var output in build.Cake.GetDirectories(build.outputDir.Path.FullPath + "/*"))
            {
                var outputName = build.artifactDir.Path + "/" + output.Segments.Last() + ".zip";
                build.Cake.Zip(output, outputName);
            }
        }

        public static void RestoreNuget(PlatformBuild build, ConvertableFilePath sln)
        {
            BuildHelper.DownloadNuget(build);

            build.Cake.Information("Restoring nuget packages for '{0}'", sln);
    
            build.Cake.NuGetRestore(sln.Path.FullPath);
        }

        public static void Clear(PlatformBuild build)
        {
            build.Cake.CleanDirectory(build.artifactDir);
            build.Cake.CleanDirectory(build.outputDir);
        }

        private static void DownloadNuget(PlatformBuild build)
        {
            var nuget = build.toolsDir + build.Cake.File("nuget.exe");
            if (!build.Cake.FileExists(nuget))
            {
                build.Cake.CreateDirectory(build.toolsDir);
                build.Cake.DownloadFile(NugetDownloadUrl, nuget);
            }
        }
    }
}