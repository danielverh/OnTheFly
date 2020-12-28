﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OnTheFly.Code;
using OnTheFly.Vm;
using OnTheFly.Vm.Runtime;

namespace OnTheFly.Vm
{
    public partial class VirtualMachine
    {
        public Instructions Instructions;

        public StringConstants constants;

        // TODO: Add garbage collector
        public static List<object> Heap = new List<object>();
        public CodeContexts DebugContexts;
        private Stack<FObject> opStack;
        private Stack<FCall> callStack;
        private Stack<FBlock> blockStack;
        private int pc;
        private Dictionary<string, FObject> globals = new Dictionary<string, FObject>();
        private Functions functions = new Functions(255);
        private TextReader consoleIn;
        private TextWriter consoleOut;

        public VirtualMachine(Instructions _instructions, CodeContexts _contexts, TextReader _in, TextWriter _out) :
            this(_instructions, _contexts)
        {
            consoleIn = _in;
            consoleOut = _out;
        }

        public VirtualMachine(Instructions _instructions, CodeContexts _contexts)
        {
            DebugContexts = _contexts;
            Instructions = _instructions;
            constants = new StringConstants(_instructions.StringConstants.ToArray());
            opStack = new Stack<FObject>(1024);
            callStack = new Stack<FCall>(128);
            blockStack = new Stack<FBlock>(256);
            blockStack.Push(new FBlock(globals));
            callStack.Push(new FCall(null, 0));
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            consoleIn = Console.In;
            consoleOut = Console.Out;
        }

        /// <summary>
        /// Runs the virtual machine
        /// </summary>
        public void Run()
        {
#if !DEBUG

            try
            {
#endif
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
                while (pc < Instructions.Count)
                {
                    OpCode cCode = NextOperation();
                    switch (cCode)
                    {
                        case OpCode.LOAD_I32:
                            opStack.Push(FObject.NewI32(NextInt()));
                            break;
                        case OpCode.LOAD_F32:
                            var f = NextFloat();
                            opStack.Push(FObject.NewF32(f));
                            break;
                        case OpCode.LOAD_BOOL:
                            opStack.Push(FObject.NewBool(NextInt() == 1));
                            break;
                        case OpCode.LOAD_STR:
                            Heap.Add(constants[NextInt()]);
                            opStack.Push(FObject.NewString(Heap.Count - 1));
                            break;
                        case OpCode.LOAD_NIL:
                            opStack.Push(FObject.Nil());
                            break;
                        case OpCode.SET_VAR:
                            blockStack.Peek()[constants[NextInt()]] = opStack.Pop();
                            break;
                        case OpCode.GET_VAR:
                            var lc = NextInt();
                            opStack.Push(blockStack.Peek()[constants[lc]]);
                            break;
                        case OpCode.ADD:
                            opStack.Push(opStack.Pop() + opStack.Pop());
                            break;
                        case OpCode.SUB:
                            opStack.Push(opStack.Pop() - opStack.Pop());
                            break;
                        case OpCode.MUL:
                            opStack.Push(opStack.Pop() * opStack.Pop());
                            break;
                        case OpCode.DIV:
                            opStack.Push(opStack.Pop() / opStack.Pop());
                            break;
                        case OpCode.MOD:
                            opStack.Push(FObject.NewI32(opStack.Pop().Int() % opStack.Pop().Int()));
                            break;
                        case OpCode.ADD_I1:
                            opStack.Push(opStack.Pop() + FObject.NewI32(1));
                            break;
                        case OpCode.SUB_I1:
                            opStack.Push(opStack.Pop() - FObject.NewI32(1));
                            break;
                        case OpCode.UNINV:
                            opStack.Push(!opStack.Pop());
                            break;
                        case OpCode.EQUALS:
                            opStack.Push(FObject.NewBool(opStack.Pop().Equals(opStack.Pop())));
                            break;
                        case OpCode.NOT_EQ:
                            opStack.Push(!FObject.NewBool(opStack.Pop().Equals(opStack.Pop())));
                            break;
                        case OpCode.SMALLER:
                            opStack.Push(opStack.Pop() < opStack.Pop());
                            break;
                        case OpCode.LARGER:
                            opStack.Push(opStack.Pop() > opStack.Pop());
                            break;
                        case OpCode.SMALLER_EQ:
                            opStack.Push(opStack.Pop() <= opStack.Pop());
                            break;
                        case OpCode.LARGER_EQ:
                            opStack.Push(opStack.Pop() >= opStack.Pop());
                            break;
                        case OpCode.JMP:
                            pc = NextInt();
                            break;
                        case OpCode.ADD_FUNCTION:
                            var name = constants[NextInt()];
                            var function = new FFunction() {Arity = NextInt(), Name = name};
                            for (int j = 0; j < function.Arity; j++)
                                function.Arguments.Add(constants[NextInt()]);
                            var endPos = NextInt();
                            function.Start = pc;
                            function.End = endPos;
                            functions[name] = function;

                            pc = endPos;
                            break;
                        case OpCode.CALL_FUNCTION:
                            name = constants[NextInt()];
                            functions.Exists(name);
                            function = functions[name];
                            for (int j = 0; j < function.Arity; j++)
                            {
                                globals[function.Arguments[j]] = opStack.Pop();
                            }

                            callStack.Push(new FCall(function, pc));
                            pc = function.Start;
                            break;
                        case OpCode.CALL_BUILTIN:
                            var lib = constants[NextInt()];
                            var method = constants[NextInt()];
                            var arity = libraries[lib].ArityMap[method];
                            var args = new FObject[arity];
                            for (int i = 0; i < arity; i++)
                                args[i] = opStack.Pop();
                            var res = libraries[lib].Invoke(method, args.Reverse().ToArray());
                            opStack.Push(res);
                            break;
                        case OpCode.RETURN:
                            var call = callStack.Pop();
                            pc = call.CalledAt;
                            // blockStack.Pop().Close();
                            // END_BLOCK is now required after RETURN
                            break;
                        case OpCode.JMP_EQ:
                            if (opStack.Pop().True())
                                pc = NextInt();
                            else
                                pc++;
                            break;
                        case OpCode.JMP_FALSE:
                            if (!opStack.Pop().True())
                                pc = NextInt();
                            else
                                pc++;
                            break;
                        case OpCode.CLONE:
                            opStack.Push(opStack.Peek());
                            break;
                        case OpCode.PRINT:
                            consoleOut.Write(opStack.Pop());
                            break;
                        case OpCode.READ_LN:
                            opStack.Push(FObject.NewString(consoleIn.ReadLine()));
                            break;
                        case OpCode.PRINT_LN:
                            consoleOut.WriteLine();
                            break;
                        case OpCode.START_BLOCK:
                            blockStack.Push(new FBlock(globals));
                            break;
                        case OpCode.START_LOOP:
                            blockStack.Push(new FBlock(globals, true, NextInt()));
                            break;
                        case OpCode.END_BLOCK:
                            blockStack.Pop().Close();
                            break;
                        case OpCode.ARRAY_ADD:
                            opStack.Push(FObject.NewArray(new FArray(24)));
                            break;
                        case OpCode.ARRAY_ADD_BIG:
                            opStack.Push(FObject.NewArray(
                                new FArray(opStack.Pop().Int(), opStack.Pop().Int())
                            ));
                            break;
                        case OpCode.ARRAY_GET:
                            var index = opStack.Pop();
                            var arr = opStack.Pop().Array();
                            opStack.Push(arr.Get(index.I32));
                            break;
                        case OpCode.ARRAY_PUSH:
                            var val = opStack.Pop();
                            var ptr = opStack.Pop().PTR;
                            ((FArray) Heap[ptr]).Push(val);
                            break;
                        case OpCode.ARRAY_SPLICE:
                            var to = opStack.Pop(); //
                            var from = opStack.Pop(); // both ints
                            arr = opStack.Pop().Array();
                            opStack.Push(FObject.NewArray(new FArray(arr, from.I32, to.I32)));
                            break;
                        case OpCode.ARRAY_SET:
                            arr = opStack.Pop().Array();
                            index = opStack.Pop();
                            val = opStack.Pop();
                            arr.items[index.I32] = val;
                            break;
                        case OpCode.ARRAY_REMOVE:
                            index = opStack.Pop(); // has to be int
                            var ptrObj = opStack.Pop();
                            arr = ptrObj.Array();
                            arr.Remove(index.I32);
                            arr.pos--;
                            opStack.Push(ptrObj);
                            break;
                        case OpCode.ARRAY_INSERT:
                            index = opStack.Pop();
                            val = opStack.Pop();
                            arr = opStack.Pop().Array();
                            arr.Insert(index.IsInt(), val);
                            break;
                        case OpCode.COUNT:
                            opStack.Push(opStack.Pop().Count());
                            break;
                        case OpCode.BREAK:
                            while (true)
                            {
                                var v = blockStack.Pop();
                                if (v.Loop)
                                {
                                    pc = v.EndPos;
                                    v.Close();
                                    break;
                                }
                                else
                                    v.Close();
                            }

                            break;
                        case OpCode.NO_OP:
                            break;
                        case OpCode.IMPORT:
                            Import(constants[NextInt()]);
                            break;
                    }
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                var ctx = DebugContexts.Where(x => pc < x.OpCodeEnd && pc > x.OpCodeStart)
                    .OrderBy(x => x.OpCodeEnd - x.OpCodeStart).FirstOrDefault();
                if (ctx != null)
                    throw new Exception(
                        $"Error '{e.GetType().Name}' at code '{ctx.Code}' (line: {ctx.Line}, pos: {ctx.Position}).\n" +
                        $"At bytecode {ctx.OpCodeStart}-{ctx.OpCodeEnd}.", e);
                throw;
            }
#endif
        }

        private Dictionary<string, FLibrary> libraries = new Dictionary<string, FLibrary>(16);

        private void Import(string package)
        {
            switch (package)
            {
                case "math":
                    libraries[package] = new MathLib();
                    break;
                case "text":
                    libraries[package] = new TextLib();
                    break;
            }
        }

        private OpCode NextOperation()
        {
            return (OpCode) Instructions[pc++];
        }

        /// <summary>
        /// Take 4 bytes from the instructions, move the pc by 4, and convert those bytes into an integer
        /// </summary>
        /// <returns></returns>
        private int NextInt()
        {
            return BitConverter.ToInt32(new[]
                {Instructions[pc++], Instructions[pc++], Instructions[pc++], Instructions[pc++]});
        }
        /// <summary>
        /// Take 4 bytes from the instructions, move the pc by 4, and convert those bytes into a float
        /// </summary>
        /// <returns></returns>
        private float NextFloat()
        {
            return BitConverter.ToSingle(new[]
                {Instructions[pc++], Instructions[pc++], Instructions[pc++], Instructions[pc++]});
        }
        /// <summary>
        /// Execute <c>Run()</c> on current VirtualMachine instance
        /// </summary>
        /// <returns>The operation stack topmost item, or Nil if there are no items on the stack</returns>
        public FObject EvalRun()
        {
            Run();
            if (opStack.Count > 0)
                return opStack.Pop();
            return FObject.Nil();
        }
    }
}