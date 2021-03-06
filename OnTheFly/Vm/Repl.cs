﻿using System;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using FlyLang;
using OnTheFly.Vm.Runtime;

namespace OnTheFly.Vm
{
    public partial class VirtualMachine
    {
        public class Repl
        {
            public bool AutoEOL { get; set; } = true;
            public TextWriter Out { get; }
            public TextReader In { get; }
            private VirtualMachine vm;
            private Listener listener = new Listener();
            private Listener _prev = null;
            public Repl(TextReader _in, TextWriter _out)
            {
                In = _in;
                Out = _out;
            }

            public Repl()
            {
                In = Console.In;
                Out = Console.Out;
            }

            public void Run()
            {
                while (true)
                {
                    Console.Write(">>> ");
                    var input = ReadInput();
                    var lexer = new FlyLexer(new AntlrInputStream(input));
                    try
                    {
                        _prev = (Listener) listener.Clone();
                        var parser = new FlyParser(new CommonTokenStream(lexer)) { ErrorHandler = new BailErrorStrategy() };
                        listener.EnterProgram(parser.program());
                    }
                    catch (Exception e)
                    {
                        listener = _prev;
                        Console.WriteLine(e.Message.Split('\n').First());
                    }

                    if (vm == null)
                        vm = new VirtualMachine(listener.Code.Instructions, listener.Code.Contexts, In, Out);
                    else
                    {
                        vm.Instructions = listener.Code.Instructions;
                        vm.DebugContexts = listener.Code.Contexts;
                    }

                    vm.constants = new StringConstants(listener.Code.Instructions.StringConstants.ToArray());
#if !DEBUG
                    try
                    {
#endif
                    var res = vm.EvalRun();
                    Console.WriteLine(res);
#if !DEBUG
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: " + e.Message);
                    }
#endif
                }
            }

            private string ReadInput()
            {
                var s = In.ReadLine();
                if (AutoEOL && (!s.EndsWith('}') && !s.EndsWith(';')))
                    s += ';';

                return s;
            }
        }
    }
}