using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm
{
    public class FCall
    {
        public FCall(FFunction function, int pc, int stackCount)
        {
            CalledAt = pc;
            Function = function;
            StackCount = stackCount;
        }
        /// <summary>
        /// Prevents function from popping items from the opStack from outside the FCall 
        /// </summary>
        public int StackCount { get; set; }
        public int CalledAt { get; set; }
        public int ContextStart { get; set; }
        public int ContextEnd { get; set; }
        // Add stack count checker
        public FFunction Function { get; set; }
    }

    public class FAnonymousCall : FCall
    {
        public FAnonymousCall(FAnonymous function, int pc, int stackCount) : base(function, pc, stackCount)
        {
        }
    }
}
