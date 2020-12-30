using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FlyLang;
using OnTheFly.Code;
using OnTheFly.Vm;

namespace OnTheFly
{
    public class FlyCode
    {
        public static (Instructions Instructions, CodeContexts Contexts) Parse(string text)
        {
            var lexer = new FlyLexer(new AntlrInputStream(text));
            var parser = new FlyParser(new CommonTokenStream(lexer));
            var listener = new Listener();
            listener.EnterProgram(parser.program());
            return (listener.Code.Instructions, listener.Code.Contexts);
        }

        public static (Instructions Instructions, CodeContexts Contexts) ParseFile(string file)
        {
            var lexer = new FlyLexer(new AntlrFileStream(file));
            var parser = new FlyParser(new CommonTokenStream(lexer));
            var listener = new Listener();
            listener.EnterProgram(parser.program());
            return (listener.Code.Instructions, listener.Code.Contexts);
        }

        public static FObject RunEval(Instructions instructions, CodeContexts contexts)
        {
            VirtualMachine vm = new VirtualMachine(instructions, contexts);
            return vm.EvalRun();
        }
    }
}
