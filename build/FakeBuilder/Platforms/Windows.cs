using FSharpx;
using System;
using System.Linq;

using static Fake.RestorePackageHelper;
using static Fake.TargetHelper;
using static Fake.MSBuildHelper;
using static Fake.VSTest;
using System.IO;
using System.Collections;
using FSharpx.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FakeBuilder.Helpers;

namespace FakeBuilder
{
    public class Windows : PlatformBase
    {
        protected string platform;
        protected string fileVersion;

        public Windows()
        {
            Target("setup", FSharpFunc.FromAction(() =>
            {
                Clear();
                GitLfs(this.Args);
                NugetRestore(ConfigHelper.Instance.SlnPath);
            }));

            Target("build", FSharpFunc.FromAction(() =>
            {
                AddDirective(this.Args);
                BuildProject(ConfigHelper.Instance.SlnPath, this.configuration, this.platform);
            }));

            Target("buildcsproj", FSharpFunc.FromAction(() =>
            {
                AddDirective(this.Args);
                BuildProject(ConfigHelper.Instance.CsprojPath, this.configuration, this.platform);
            }));

            Target("test", FSharpFunc.FromAction(() =>
            {
                RunTest();
            }));

            Target("upload", FSharpFunc.FromAction(() =>
            {
                GenerateArtifacts();
                ZipAndUploadToTeamCity();
            }));

            //dependency("Build", "Clean");
        }

        protected void NugetRestore(string sln)
        {
            Nuget3Restore(nrParams => { return nrParams; }, sln);
        }

        protected void BuildProject(string sln, string configuration, string platform)
        {
            //Updates version before compiling
            this.UpdateAssemblyVersion();
            MSBuildLoggers = (new string[] { }).ToFSharpList();
            build(Fun<MSBuildParams>(msBuild =>
            {
                msBuild.Verbosity = MSBuildVerbosity.Detailed.ToFSharpOption();
                msBuild.NoLogo = true;
                msBuild.Properties = this.propperties.ToArray().ToFSharpList();
                return msBuild;
            }), sln);
        }

        protected void UpdateAssemblyVersion()
        {
            var pathToProject = Path.GetDirectoryName(ConfigHelper.Instance.CsprojPath);
            var pathToAssembly = Path.GetFullPath($"{pathToProject}/Properties/AssemblyInfo.cs");
            string text = File.ReadAllText(pathToAssembly);
            text = Regex.Replace(text, "AssemblyFileVersion\\(\"(.*)\"\\)", $"AssemblyFileVersion(\"{this.fileVersion}\")");
            File.WriteAllText(pathToAssembly, text);
        }

        public override void InitCommon()
        {
            base.InitCommon();
            this.configuration = "Release";
            this.platform = "x64";
            this.fileVersion = $"{ConfigHelper.Instance.MajorVersion}.{ConfigHelper.Instance.MinorVersion}.{buildVersion}";

            this.propperties.Add(new Tuple<string, string>("Configuration", configuration));
            this.propperties.Add(new Tuple<string, string>("Platform", platform));
        }

        public override void RunTest()
        {
            VSTestParams param = new VSTestParams() { SettingsPath = $"{this.solutionOutput}/{ConfigHelper.Instance.TestProjectName}.runsettings" };
            var testsAssemblies = Directory.GetFiles(this.solutionOutput, "*Tests.dll");
            VSTest(Fun<VSTestParams>(vtest =>
            {
                vtest.SettingsPath = $"{this.solutionOutput}/{ConfigHelper.Instance.TestProjectName}.runsettings";
                return vtest;
            }), testsAssemblies);
        }
    }
}
