using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Vm.Runtime
{
    public class FunctionInvoker
    {
        private VirtualMachine vm;
        public FunctionInvoker(VirtualMachine _vm)
        {
            vm = _vm;
        }

        public FObject Invoke(FAnonymous function, FObject[] args)
        {
            vm.SetInvokeArguments(function.Arguments.ToArray(), args);
            return vm.Invoke(vm.immutableInstr, function, args);
        }
    }
}
