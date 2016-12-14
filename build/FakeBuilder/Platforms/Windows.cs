
using Fake;
using FSharpx;
using System;

namespace FakeBuilder
{
    public class Windows : PlatformBuild
    {
        public Windows()
        {
            TargetHelper.Target("Default", FSharpFunc.FromAction(() => 
            {
                Console.WriteLine("Windows");
            }));
        }
    }
}
