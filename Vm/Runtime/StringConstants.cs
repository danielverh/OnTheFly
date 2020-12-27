using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheFly.Vm.Runtime.Exceptions;

namespace OnTheFly.Vm.Runtime
{
    public class StringConstants
    {
        public StringConstants(string[] arr)
        {
            constants = arr;
        }
        private string[] constants;

        public string this[int index]
        {
            get
            {
                if (index > constants.Length)
                {
                    throw new StringConstantNotFoundException(index);
                }
                return constants[index];
            }
            set
            {
                /* set the specified index to value here */
            }
        }
    }
}
