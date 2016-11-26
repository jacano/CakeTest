using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.NuGet;
using Cake.Common.Diagnostics;
using Cake.Common.Net;
using Cake.Core;
using Cake.Core.Diagnostics;

using CodeCake;
using Cake.Common.Solution;
using Cake.Core.IO;

namespace CakeBuilder
{
    //[AddPath("tools")]
    public class Build : CodeCakeHost
    {
        private const string NugetDownloadUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

        public Build()
        {
            var configuration = Cake.Argument("configuration", "Release");

            var artifactDir = Cake.Directory("artifacts");
            var outputDir = Cake.Directory("output");
            var toolsDir = Cake.Directory("tools");

            Task("Clean").Does(() =>
            {
                Cake.CleanDirectory(artifactDir);
                Cake.CleanDirectory(outputDir);
            });

            Task("Nuget").Does(() =>
            {
                var nuget = toolsDir + Cake.File("nuget.exe");
                if (!Cake.FileExists(nuget))
                {
                    Cake.CreateDirectory(toolsDir);
                    Cake.DownloadFile(NugetDownloadUrl, nuget);
                }
            });

            Task("RestoreNuGetPackages")
            .IsDependentOn("Nuget")
            .Does(() =>
            {
                Cake.Information("Restoring nuget packages for existing .sln files.");
                foreach (var sln in Cake.GetFiles("src/*.sln"))
                {
                    Cake.NuGetRestore(sln);
                }
            });

            Task("Build")
            .IsDependentOn("Clean")
            .IsDependentOn("RestoreNuGetPackages")
            .Does(() =>
            {
                Cake.Information("Building all existing .sln files at the root level with '{0}' configuration (excluding this builder application).", configuration);
                foreach (var sln in Cake.GetFiles("*.sln"))
                {
                    using (var tempSln = Cake.CreateTemporarySolutionFile(sln))
                    {
                        // Excludes "CodeCakeBuilder" itself from compilation!
                        tempSln.ExcludeProjectsFromBuild("CakeBuilder");
                        Cake.MSBuild(tempSln.FullPath, new MSBuildSettings()
                                .SetConfiguration(configuration)
                                .SetVerbosity(Verbosity.Minimal)
                                .SetMaxCpuCount(1));
                    }
                }
            });

            Task("Default")
            .IsDependentOn("Build");
        }
    }
}
