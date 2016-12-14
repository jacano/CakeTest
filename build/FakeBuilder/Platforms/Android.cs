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

                MSBuildHelper.build(Fun<MSBuildHelper.MSBuildParams>(msBuild => {
                    msBuild.Verbosity = MSBuildHelper.MSBuildVerbosity.Detailed.ToFSharpOption();
                    msBuild.NoLogo = true;
                    msBuild.RestorePackagesFlag = true;
                    return msBuild;
                }), $"a/ScriptingDemos.csproj");
            }));
        }
    }
}
