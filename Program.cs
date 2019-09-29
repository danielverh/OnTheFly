using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FlyLang;
using OnTheFly.Vm;

namespace OnTheFly
{
    class Program
    {
        static void Main(string[] args)
        {
            var debug = false;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var lexer = new FlyLexer(new AntlrFileStream("test.txt"));
            var parser = new FlyParser(new CommonTokenStream(lexer));
            var listener = new Listener();
            listener.EnterProgram(parser.program());
            var disassemble = listener.Instructions.Disassemble();
            if (debug)
                Console.Write(disassemble);

            Console.WriteLine("Starting VM... ");
            Console.ForegroundColor = ConsoleColor.Green;
            var vm = new VirtualMachine(listener.Instructions);
//            try
//            {
            var sw = Stopwatch.StartNew();
            vm.Run();
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"\n----\nVirtualMachine.Run() took {sw.ElapsedMilliseconds}ms");
//            }
//            catch (Exception e)
//            {
//                new RuntimeErrorHandler(e);
//                throw e;
//            }
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}