using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FlyLang;
using OnTheFly.Code;
using OnTheFly.Vm;

namespace OnTheFly
{
    class Program
    {
        static void Main(string[] args)
        {
            var repl = new VirtualMachine.Repl();
            repl.Run();
        }
    }
}