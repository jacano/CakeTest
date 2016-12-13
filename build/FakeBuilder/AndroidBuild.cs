using System;
using Fake;
using FSharpx;

namespace FakeBuilder
{
    public class AndroidBuild : PlatformBuild
    {
        static Microsoft.FSharp.Core.FSharpFunc<TParams, TParams> Fun<TParams>(Func<TParams, TParams> func) => FSharpx.FSharpFunc.FromFunc(func);

        public AndroidBuild()
        {
            TargetHelper.Target("Default", FSharpFunc.FromAction(() => {
                Console.WriteLine("Woohoo, nothing to do!");
            }));

            TargetHelper.run("Default");

            /*build(Fun<MSBuildParams>(msBuild => {
                msBuild.Verbosity = MSBuildVerbosity.Detailed.ToFSharpOption();
                msBuild.NoLogo = true;
                msBuild.RestorePackagesFlag = true;
                return msBuild;
            }), $"a/ScriptingDemos.csproj");


            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            BuildHelper.GenerateArtifactsAndMove(this);

            BuildHelper.UploadToTeamCity(this);*/
        }
    }
}
