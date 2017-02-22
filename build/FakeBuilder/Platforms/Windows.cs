using FSharpx;
using static Fake.RestorePackageHelper;
using static Fake.TargetHelper;
using static Fake.MSBuildHelper;
using System.IO;
using System;

namespace FakeBuilder
{
    public class Windows : PlatformBase
    {
        public Windows()
        {
            var sln = $"src/WaveCity3D/WaveCity3D_Windows.sln";
            var configuration = "Release";
            var platform = "x86";

            Target("ci", FSharpFunc.FromAction(() => 
            {
                InitCommon(sln);
                Clear();
                NugetRestore(sln);
                BuildProject(sln, configuration, platform);
            }));

            Target("cd", FSharpFunc.FromAction(() =>
            {
                InitCommon(sln);
                Clear();
                NugetRestore(sln);
                BuildProject(sln, configuration, platform);
                GenerateArtifacts();
                UploadToTeamCity();
            }));

            //dependency("Build", "Clean");
        }

        private void NugetRestore(string sln)
        {
            Nuget3Restore(nrParams => { return nrParams; }, sln);
        }

        private void BuildProject(string sln, string configuration, string platform)
        {
            MSBuildLoggers = (new string[] { }).ToFSharpList();
            build(Fun<MSBuildParams>(msBuild => {
                msBuild.Verbosity = MSBuildVerbosity.Detailed.ToFSharpOption();
                msBuild.NoLogo = true;
                msBuild.Properties = (new[] 
                {
                    new Tuple<string, string>("OutputPath", this.solutionOutput),
                    new Tuple<string, string>("Configuration", configuration),
                    new Tuple<string, string>("Platform", platform),
                }).ToFSharpList();
                return msBuild;
            }), sln);
        }
    }
}
