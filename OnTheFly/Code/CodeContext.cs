using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Code
{
    [Serializable]
    public class CodeContext
    {
        public string Line { get; set; }
        public string Code { get; set; }
        public int OpCodeStart { get; set; }
        public int OpCodeEnd { get; set; }
        public string Position { get; set; }
    }
    [Serializable]
    public class CodeContexts : List<CodeContext>{}
}
