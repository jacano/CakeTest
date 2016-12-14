
using System;
using Fake;

namespace FakeBuilder
{
    public class PlatformBuild 
    {
        public string buildNumber;
        public string buildName;

        public static Microsoft.FSharp.Core.FSharpFunc<TParams, TParams> Fun<TParams>(Func<TParams, TParams> func) => FSharpx.FSharpFunc.FromFunc(func);

        public void Run(string targetName)
        {
            TargetHelper.run(targetName);
        }
    }
}