using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm
{
    public class FFunction
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Arity { get; set; }
        public string Name { get; set; }
        public List<string> Arguments { get; set; } = new List<string>();
    }
}
