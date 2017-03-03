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
using System.Linq;

namespace FakeBuilder
{
    public abstract class PlatformBase
    {
        protected string configuration;

        public string buildName;
        public string buildVersion;
        public string solutionName;
        public string solutionOutput;

        public IEnumerable<string> Args;
        protected List<Tuple<string, string>> propperties;

        public static Microsoft.FSharp.Core.FSharpFunc<TParams, TParams> Fun<TParams>(Func<TParams, TParams> func) => FSharpx.FSharpFunc.FromFunc(func);

        public PlatformBase()
        {
            this.InitCommon();
        }

        public virtual void InitCommon()
        {
            this.solutionName = Path.GetFileNameWithoutExtension(ConfigHelper.Instance.SlnPath);
            this.buildName = environVarOrNone("VersionName").ValueOrDefault("1.0");
            this.buildVersion = TeamCityBuildNumber.ValueOrDefault("1");
            this.solutionOutput = Path.Combine(Path.GetFullPath(ConfigHelper.Instance.OutputFolder), this.solutionName + "_" + this.buildVersion) + "/";

            this.propperties = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("OutputPath", this.solutionOutput),
                    new Tuple<string, string>("AllowUnsafeBlocks", "true"),
                };
        }

        public virtual void Nuget3Restore(Func<RestorePackageParams, RestorePackageParams> setParams, string sln)
        {
            var parameters = setParams(RestorePackageDefaults);
            var sources = buildSources(parameters.Sources);
            var args = "\"restore\" \"" + sln + "\"" + sources;
            runNuGetTrial(parameters.Retries, parameters.ToolPath, parameters.TimeOut, args, Fun<Unit>((o) => { throw new Exception($"Package restore of {sln} failed"); }));
        }


        public virtual void ZipAndUploadToTeamCity()
        {
            var folders = Directory.EnumerateDirectories(ConfigHelper.Instance.ArtifactsFolder, "*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                var filesInFolder = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
                var folderFileInfo = new DirectoryInfo(folder);
                var zipFilename = folderFileInfo.Name + ".zip";
                var zipLocation = Path.Combine(ConfigHelper.Instance.ArtifactsFolder, zipFilename);
                Zip(folder, zipLocation, filesInFolder);
            }

            UploadToTeamCity();
        }

        public virtual void UploadToTeamCity()
        {
            var files = Directory.EnumerateFiles(ConfigHelper.Instance.ArtifactsFolder, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileFullPath = Path.GetFullPath(file);
                PublishArtifact(fileFullPath);
            }
        }

        public virtual void GenerateArtifacts()
        {
            var artifactOutput = Path.Combine(Path.GetFullPath(ConfigHelper.Instance.ArtifactsFolder), solutionName + "_" + this.buildVersion) + "/";

            if (!Directory.Exists(ConfigHelper.Instance.ArtifactsFolder))
            {
                Directory.CreateDirectory(ConfigHelper.Instance.ArtifactsFolder);
            }

            if (Directory.Exists(artifactOutput))
            {
                Directory.Delete(artifactOutput, true);
            }

            Directory.Move(this.solutionOutput, artifactOutput);
        }

        public virtual void RunTest()
        {
        }

        public virtual void Clear()
        {
            if (Directory.Exists(ConfigHelper.Instance.OutputFolder))
            {
                Directory.Delete(ConfigHelper.Instance.OutputFolder, true);
            }

            if (Directory.Exists(ConfigHelper.Instance.ArtifactsFolder))
            {
                Directory.Delete(ConfigHelper.Instance.ArtifactsFolder, true);
            }
        }

        public virtual void GitLfs(IEnumerable<string> args)
        {
            if (args.Count() < 2)
            {
                return;
            }
            var user = args.ElementAt(0);
            var pass = args.ElementAt(1);
            var url = args.ElementAt(2);

            var result = PlatfomHelper.Execute("git", $"lfs pull \"https://{user}:{pass}@{url}\"");
            if (result.Result != 0)
            {
                throw new Exception($"Not valid parameters usr:{user} pass:{pass} url:{url}");
            }
        }


        public virtual void AddDirective(IEnumerable<string> args)
        {
            if (args.Any())
            {
                this.propperties.Add(new Tuple<string, string>("DefineConstants", string.Join(";", args)));
            }
        }

        public virtual void Run(string targetName)
        {
            TargetHelper.run(targetName);
        }
    }
}