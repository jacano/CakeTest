using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.NuGet;
using Cake.Common.Diagnostics;
using Cake.Common.Net;
using Cake.Common.Build.TeamCity;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

using CodeCake;
using System.Linq;

namespace CakeBuilder
{
    //[AddPath("tools")]
    public class Build : CodeCakeHost
    {
        private const string NugetDownloadUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe";

        public Build()
        {
            var buildNumber = Cake.EnvironmentVariable("BUILDNUMBER") ?? "1";

            var artifactDir = Cake.Directory("artifacts");
            var outputDir = Cake.Directory("output");
            var toolsDir = Cake.Directory("tools");

            var configuration = Cake.Argument("Configuration", "Debug");

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
                Cake.Information("Building all existing .csproj files with '{0}' configuration", configuration);
                foreach (var csproj in Cake.GetFiles("src/**/*.csproj"))
                {
                    var outputPath = outputDir.Path.MakeAbsolute(Cake.Environment).FullPath + "/" + csproj.GetFilenameWithoutExtension().FullPath + "_" + buildNumber + "/";

                    Cake.MSBuild(csproj.FullPath,
                        new MSBuildSettings()
                        .WithProperty("OutputPath", outputPath)
                        .SetConfiguration(configuration)
                        .SetVerbosity(Verbosity.Minimal)
                        .SetMaxCpuCount(1));

                }
            });

            Task("GenerateArtifactsAndMove")
                .IsDependentOn("Build")
                .Does(() =>
                {
                    foreach (var output in Cake.GetDirectories(outputDir.Path.FullPath + "/*"))
                    {
                        var outputName = artifactDir.Path + "/" + output.Segments.Last() + ".zip";
                        Cake.Zip(output, outputName);
                    } 
                });

            Task("UploadTeamCity")
                .IsDependentOn("GenerateArtifactsAndMove")
                .Does(() =>
                {
                    var teamCityProvider = new TeamCityProvider(Cake.Environment, Cake.Log);
                    teamCityProvider.PublishArtifacts(artifactDir.Path.FullPath);
                });

            Task("Default")
            .IsDependentOn("UploadTeamCity");
        }
    }
}
