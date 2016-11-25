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

namespace CakeBuilder
{
    //[AddPath("CakeBuilder/Tools")]
    public class Build : CodeCakeHost
    {
        public Build()
        {
            var configuration = Cake.Argument("configuration", "Release");
            var releasesDir = Cake.Directory("CakeBuilder/Releases");

            Task("Clean")
                .Does(() =>
               {
                    Cake.CleanDirectories(releasesDir);
               });

            Task("Nuget")
                .Does(() =>
                {
                    var nuget = "tools/nuget.exe";
                    if (!Cake.FileExists(nuget))
                    {
                        Cake.CreateDirectory("tools/");
                        Cake.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", nuget);
                    }
                });

            Task("Restore-NuGet-Packages")
                .IsDependentOn("Nuget")
                .Does(() =>
               {
                   Cake.Information("Restoring nuget packages for existing .sln files at the root level.");
                   foreach (var sln in Cake.GetFiles("*.sln"))
                   {
                       Cake.NuGetRestore(sln);
                   }
               });

            Task("Build")
                .IsDependentOn("Clean")
                .IsDependentOn("Restore-NuGet-Packages")
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

            Task("Default").IsDependentOn("Build");
        }
    }
}
