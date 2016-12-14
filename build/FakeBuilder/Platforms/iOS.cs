
using Fake;
using FSharpx;
using System;

namespace FakeBuilder
{
    public class iOS : PlatformBuild
    {
        public iOS()
        {
            TargetHelper.Target("Default", FSharpFunc.FromAction(() => 
            {
                Console.WriteLine("iOS");
            }));
        }
    }
}
