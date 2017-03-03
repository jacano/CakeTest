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
            TargetHelper.Target("Default", FSharpFunc.FromAction(() =>
            {
                Console.WriteLine("Android");
            }));
        }
    }
}
