using System;
using System.Globalization;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FlyLang;

namespace OnTheFly
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var lexer = new FlyLexer(new AntlrFileStream("test.txt"));
            var parser = new FlyParser(new CommonTokenStream(lexer));
            var listener = new Listener();
            listener.EnterProgram(parser.program());
            listener.Instructions.Add(OpCode.PRINT);
            var vm = new VirtualMachine(listener.Instructions);
            vm.Run();
        }
    }
}
