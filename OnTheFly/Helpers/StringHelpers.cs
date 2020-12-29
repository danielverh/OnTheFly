using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Helpers
{
    public static class StringHelpers
    {
        public static string PrettyArray<T>(this IEnumerable<T> items)
        {
            return $"[{string.Join(", ", items)}]";
        }
    }
}
