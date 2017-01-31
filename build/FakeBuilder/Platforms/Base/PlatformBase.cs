using FakeBuilder.Helpers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static Fake.EnvironmentHelper;
using static Fake.TeamCityHelper;
using static Fake.ZipHelper;
using Fake;

namespace FakeBuilder
{
    public class PlatformBase 
    {
        private const string ArtifactsFolder = "artifacts";
        private const string OutputFolder = "output";

        public string buildName;
        public string buildVersion;
        public string projectName;
        public string projectOutput;

        public static Microsoft.FSharp.Core.FSharpFunc<TParams, TParams> Fun<TParams>(Func<TParams, TParams> func) => FSharpx.FSharpFunc.FromFunc(func);

        public virtual void InitCommon(string csproj)
        {
            this.buildName = environVarOrNone("VersionName").ValueOrDefault("1.0");
            this.buildVersion = TeamCityBuildNumber.ValueOrDefault("1");

            this.projectName = Path.GetFileNameWithoutExtension(csproj);

            this.projectOutput = Path.Combine(Path.GetFullPath(OutputFolder), projectName + "_" + this.buildVersion) + "/";
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
            var artifactOutput = Path.Combine(Path.GetFullPath(ArtifactsFolder), projectName + "_" + this.buildVersion) + "/";

            if (!Directory.Exists(ArtifactsFolder))
            {
                Directory.CreateDirectory(ArtifactsFolder);
            }

            if (Directory.Exists(artifactOutput))
            {
                Directory.Delete(artifactOutput, true);
            }

            Directory.Move(this.projectOutput, artifactOutput);
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