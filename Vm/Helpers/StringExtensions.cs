using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm.Helpers
{
    public static class StringExtensions
    {
        public static string Repeat(this string input, int count)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var output = "";
                for (int i = 0; i < count; i++)
                    output += input;
                return output;
            }

            return string.Empty;
        }
    }
}
