using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm
{
    public class RuntimeErrorHandler
    {
        public RuntimeErrorHandler(Exception e)
        {
            // TODO: Add line numbers etc.
            var sb = new StringBuilder();
            sb.AppendLine("There was an exception on runtime.");
            sb.AppendLine($"- {e.Source}; {e.Message}");
            Console.Write(sb.ToString());
        }
    }
}
