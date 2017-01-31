using Fake;
using FSharpx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fake.TargetHelper;
using static Fake.MSBuildHelper;

namespace FakeBuilder.Platforms
{
    public class UWP : PlatformBase
    {
        public UWP()
        {
            var sln = $"src/WaveCity3D/WaveCity3D_UWP.sln";
            var configuration = "Debug";

            Target("ci", FSharpFunc.FromAction(() =>
            {
                InitCommon(sln);
                Clear();
                NugetRestore(sln);
                BuildProject(sln, configuration);
            }));

            Target("cd", FSharpFunc.FromAction(() =>
            {
                InitCommon(sln);
                Clear();
                NugetRestore(sln);
                BuildProject(sln, configuration);
                GenerateArtifacts();
                UploadToTeamCity();
            }));
        }

        private void NugetRestore(string sln)
        {
            Nuget3Restore(nrParams => { return nrParams; }, sln);
        }

        private void BuildProject(string sln, string configuration)
        {
            MSBuildLoggers = (new string[] { }).ToFSharpList();
            build(Fun<MSBuildParams>(msBuild => {
                msBuild.Verbosity = MSBuildVerbosity.Detailed.ToFSharpOption();
                msBuild.NoLogo = true;
                msBuild.Properties = (new[]
                {
                    new Tuple<string, string>("OutputPath", this.solutionOutput),
                    new Tuple<string, string>("Configuration", configuration),
                }).ToFSharpList();
                return msBuild;
            }), sln);
        }
    }
}
