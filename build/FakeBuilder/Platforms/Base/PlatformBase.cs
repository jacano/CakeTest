using FakeBuilder.Helpers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static Fake.EnvironmentHelper;
using static Fake.TeamCityHelper;
using static Fake.ZipHelper;
using static Fake.RestorePackageHelper;
using Fake;

namespace FakeBuilder
{
    public class PlatformBase 
    {
        private const string ArtifactsFolder = "artifacts";
        private const string OutputFolder = "output";

        public string buildName;
        public string buildVersion;
        public string solutionName;
        public string solutionOutput;

        public static Microsoft.FSharp.Core.FSharpFunc<TParams, TParams> Fun<TParams>(Func<TParams, TParams> func) => FSharpx.FSharpFunc.FromFunc(func);

        public virtual void InitCommon(string sln)
        {
            this.buildName = environVarOrNone("VersionName").ValueOrDefault("1.0");
            this.buildVersion = TeamCityBuildNumber.ValueOrDefault("1");

            this.solutionName = Path.GetFileNameWithoutExtension(sln);
            this.solutionOutput = Path.Combine(Path.GetFullPath(OutputFolder), solutionName + "_" + this.buildVersion) + "/";
        }

        public virtual void Nuget3Restore(Func<RestorePackageParams, RestorePackageParams> setParams, string sln)
        {
            var parameters = setParams(RestorePackageDefaults);
            var sources = buildSources(parameters.Sources);

            var args = "\"restore\" \"" + sln + "\"" + sources;
            runNuGetTrial(parameters.Retries, parameters.ToolPath, parameters.TimeOut, args, Fun<Unit>((o) => { throw new Exception($"Package restore of {sln} failed"); }));
        }


        public virtual void UploadToTeamCity()
        {
            var folders = Directory.EnumerateDirectories(ArtifactsFolder, "*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                var filesInFolder = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
                var folderFileInfo = new DirectoryInfo(folder);
                var zipFilename = folderFileInfo.Name + ".zip";
                var zipLocation = Path.Combine(ArtifactsFolder, zipFilename);
                Zip(folder, zipLocation, filesInFolder);
            }

            var files = Directory.EnumerateFiles(ArtifactsFolder, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileFullPath = Path.GetFullPath(file);
                PublishArtifact(fileFullPath);
            }
        }

        public virtual void GenerateArtifacts()
        {
            var artifactOutput = Path.Combine(Path.GetFullPath(ArtifactsFolder), solutionName + "_" + this.buildVersion) + "/";

            if (!Directory.Exists(ArtifactsFolder))
            {
                Directory.CreateDirectory(ArtifactsFolder);
            }

            if (Directory.Exists(artifactOutput))
            {
                Directory.Delete(artifactOutput, true);
            }

            Directory.Move(this.solutionOutput, artifactOutput);
        }

        public virtual void Clear()
        {
            if (Directory.Exists(OutputFolder))
            {
                Directory.Delete(OutputFolder, true);
            }

            if (Directory.Exists(ArtifactsFolder))
            {
                Directory.Delete(ArtifactsFolder, true);
            }
        }

        public virtual void Run(string targetName)
        {
            TargetHelper.run(targetName);
        }
    }
}