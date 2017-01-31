using System;
using Fake;
using FSharpx;

namespace FakeBuilder
{
    public class Android : PlatformBase
    {
        public Android()
        {
            TargetHelper.Target("Default", FSharpFunc.FromAction(() => 
            {
                Console.WriteLine("Android");
            }));
        }
    }
}
