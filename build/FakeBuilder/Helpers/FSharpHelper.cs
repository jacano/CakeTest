using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBuilder.Helpers
{
    public static class FSharpHelper
    {
        public static bool HasValue<T>(this FSharpOption<T> option)
        {
            return FSharpOption<T>.get_IsSome(option);
        }

        public static T ValueOrDefault<T>(this FSharpOption<T> value, T defaultValue)
        {
            if (!value.HasValue())
                return defaultValue;
            return value.Value;
        }
    }
}
