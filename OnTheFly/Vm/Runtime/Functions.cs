using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheFly.Vm.Runtime.Exceptions;

namespace OnTheFly.Vm.Runtime
{
    internal class Functions : Dictionary<string, FFunction>
    {
        public Functions()
        {
        }

        public Functions(int capacity) : base(capacity)
        {
        }

        public void Exists(string name)
        {
            if(!this.ContainsKey(name))
                throw new InvalidFunctionNameException(name); 
        }
    }
}