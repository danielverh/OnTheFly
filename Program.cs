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
            args = new string[] { "test.txt" };
            var debug = true;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            if(args.Length < 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("File doesn't exist.");
                return;
            }
            var lexer = new FlyLexer(new AntlrFileStream(args[0]));
            var parser = new FlyParser(new CommonTokenStream(lexer));
            var listener = new Listener();
            listener.EnterProgram(parser.program());
            if (debug)
            {
                var disassemble = listener.Instructions.Disassemble();
                Console.Write(disassemble);
            }

            Console.WriteLine("Starting VM... ");
            Console.ForegroundColor = ConsoleColor.Green;
            var vm = new VirtualMachine(listener.Instructions, listener.Contexts);
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