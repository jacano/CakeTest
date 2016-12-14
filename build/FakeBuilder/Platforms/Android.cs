using System;
using Fake;
using FSharpx;

namespace FakeBuilder
{
    public class Android : PlatformBuild
    {
        public Android()
        {
            TargetHelper.Target("Default", FSharpFunc.FromAction(() => 
            {
                Console.WriteLine("Android");

                MSBuildHelper.MSBuildLoggers = (new string[] { }).ToFSharpList();
                MSBuildHelper.build(Fun<MSBuildHelper.MSBuildParams>(msBuild => {
                    msBuild.Verbosity = MSBuildHelper.MSBuildVerbosity.Detailed.ToFSharpOption();
                    msBuild.NoLogo = true;
                    msBuild.RestorePackagesFlag = true;
                    return msBuild;
                }), $"src/App3/App3.csproj");
            }));
        }
    }
}
