
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
            Target("Default", FSharpFunc.FromAction(() => 
            {
                var csproj = $"src/App3/App3.csproj";
                var sln = $"src/App3.sln";

                InitCommon(csproj);
                Clear();
                //NugetRestore(sln);
                BuildProject(csproj);
                GenerateArtifacts();
                UploadToTeamCity();
            }));

            //dependency("Build", "Clean");
        }

        private void NugetRestore(string sln)
        {
            RestoreMSSolutionPackages(Fun<RestorePackageParams>(nrParams => {
                nrParams.Retries = 3;
                return nrParams;
            }), sln);
        }

        private void BuildProject(string csproj)
        {
            MSBuildLoggers = (new string[] { }).ToFSharpList();
            build(Fun<MSBuildParams>(msBuild => {
                msBuild.Verbosity = MSBuildVerbosity.Detailed.ToFSharpOption();
                msBuild.NoLogo = true;
                msBuild.RestorePackagesFlag = true;
                msBuild.Properties = (new[] {new Tuple<string, string>("OutputPath", this.projectOutput)}).ToFSharpList();
                return msBuild;
            }), csproj);

        }
    }
}
