﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Vm.Runtime.Exceptions
{
    public class InvalidFunctionNameException : Exception
    {
        public InvalidFunctionNameException(string name) : base(
            $"Cannot find function with name '{name}'.")
        {
        }
    }

    public class StringConstantNotFoundException : Exception
    {
        public StringConstantNotFoundException(int index) : base($"String constant with index {index} was not found.")
        {
        }
    }

    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base("Runtime exception: " + message){}
    }
}