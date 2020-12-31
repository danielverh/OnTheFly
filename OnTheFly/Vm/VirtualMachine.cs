using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OnTheFly.Code;
using OnTheFly.Vm;
using OnTheFly.Vm.Runtime;
using OnTheFly.Vm.Runtime.Exceptions;

namespace OnTheFly.Vm
{
    public partial class VirtualMachine
    {
        private Instructions _instr;

        public Instructions Instructions
        {
            get => _instr;
            set => immutableInstr = ImmutableArray.CreateRange(_instr = value);
        }

        public ImmutableArray<byte> immutableInstr;

        public StringConstants constants;
        public static MemoryHeap Heap = new MemoryHeap();

        public CodeContexts DebugContexts;
        private Stack<FObject> opStack;
        private Stack<FCall> callStack;
        private Stack<FBlock> blockStack;
        private int pc;
        private Dictionary<string, FObject> globals = new Dictionary<string, FObject>();
        private Functions functions = new Functions(255);
        private TextReader consoleIn;
        private TextWriter consoleOut;
        // For debugging:
        private OpCode previousOpCode;
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
            blockStack.Push(new FBlock(globals, 0, null));
            callStack.Push(new FCall(null, 0, 0));
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
            FObject res, indexObj;
            int index;
            string key;
            while (pc < immutableInstr.Length)
            {
                OpCode cCode = NextOperation();
                switch (cCode)
                {
                    case OpCode.LOAD_I64:
                        opStack.Push(FObject.NewI64(NextLong()));
                        break;
                    case OpCode.LOAD_I32:
                        opStack.Push(FObject.NewI64(NextInt()));
                        break;
                    case OpCode.LOAD_F64:
                        var f = NextDouble();
                        opStack.Push(FObject.NewF64(f));
                        break;
                    case OpCode.LOAD_BOOL:
                        opStack.Push(FObject.NewBool(NextOperation() != OpCode.NO_OP));
                        break;
                    case OpCode.LOAD_STR:
                        opStack.Push(FObject.NewString(Heap.Add(constants[NextInt()])));
                        break;
                    case OpCode.LOAD_NIL:
                        opStack.Push(FObject.Nil());
                        break;
                    case OpCode.POP:
                        opStack.Pop();
                        break;
                    case OpCode.SET_VAR:
                        index = NextInt();
                        key = constants[index];
                        if (blockStack.Peek().Contains(key))
                            Garbage.Mark(blockStack.Peek()[key]);
                        res = blockStack.Peek()[constants[index]] = opStack.Pop();
                        opStack.Push(res);
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
                        opStack.Push(opStack.Pop() % opStack.Pop());
                        break;
                    case OpCode.ADD_I1:
                        opStack.Push(opStack.Pop() + FObject.NewI64(1));
                        break;
                    case OpCode.SUB_I1:
                        opStack.Push(opStack.Pop() - FObject.NewI64(1));
                        break;
                    case OpCode.UNINV:
                        opStack.Push(!opStack.Pop());
                        break;
                    case OpCode.UN_NEGATIVE:
                        opStack.Push(-opStack.Pop());
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
                        var function = new FFunction() { Arity = NextInt(), Name = name };
                        for (int j = 0; j < function.Arity; j++)
                            function.Arguments.Add(constants[NextInt()]);
                        var endPos = NextInt();
                        function.Start = pc;
                        function.End = endPos;
                        functions[name] = function;

                        pc = endPos;
                        break;
                    case OpCode.CALL_BUILTIN:
                        name = constants[NextInt()];
                        var builtin = Builtins.GetBuiltin(name);
                        var inputs = new List<FObject>();
                        for (int i = 0; i < builtin.Arity + 1; i++)
                            inputs.Add(opStack.Pop());
                        callStack.Push(new FCall(builtin, pc, opStack.Count));
                        inputs.Reverse();
                        opStack.Push(builtin.Invoke(inputs.ToArray()));
                        callStack.Pop();

                        break;
                    case OpCode.CALL_FUNCTION:
                        name = constants[NextInt()];
                        functions.Exists(name);
                        function = functions[name];
                        for (int j = 0; j < function.Arity; j++)
                        {
                            // TODO: Should be in top blockstack
                            blockStack.Peek()[function.Arguments[j]] = opStack.Pop();
                        }

                        callStack.Push(new FCall(function, pc, opStack.Count));
                        pc = function.Start;
                        break;
                    case OpCode.CALL_LIBFUNC:
                        var lib = constants[NextInt()];
                        var method = constants[NextInt()];
                        var arity = libraries[lib].ArityMap[method];
                        var args = new FObject[arity];
                        if (arity > opStack.Count)
                            throw new RuntimeException(
                                $"Not enough items on the operator stack to fill arguments of function '{lib}.{method}()'. " +
                                $"Argument count is {args}"
                            );
                        for (int i = 0; i < arity; i++)
                            args[i] = opStack.Pop();
                        res = libraries[lib].Invoke(method, args.Reverse().ToArray());
                        opStack.Push(res);
                        break;
                    case OpCode.RETURN:
                        var call = callStack.Pop();
                        var ret = opStack.Pop();
                        while (opStack.Count > call.StackCount)
                        {
                            opStack.Pop();
                        }

                        opStack.Push(ret);
                        pc = call.CalledAt;
                        // blockStack.Pop().Close();
                        // END_BLOCK is now required after RETURN
                        break;
                    case OpCode.JMP_EQ:
                        int offset = NextInt();
                        if (opStack.Pop().True())
                            pc = offset;
                        break;
                    case OpCode.JMP_FALSE:
                        offset = NextInt();
                        if (!opStack.Pop().True())
                            pc = offset;
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
                        blockStack.Push(new FBlock(globals, opStack.Count, blockStack.Peek()));
                        break;
                    case OpCode.START_LOOP:
                        blockStack.Push(new FBlock(globals, opStack.Count, blockStack.Peek(), true, NextInt()));
                        break;
                    case OpCode.END_BLOCK:
                        var endBlock = blockStack.Pop();
                        while (opStack.Count > endBlock.StackCount)
                        {
                            opStack.Pop();
                        }

                        endBlock.Close();
                        break;
                    case OpCode.ARRAY_ADD:
                        opStack.Push(FObject.NewArray(new FArray(24)));
                        break;
                    case OpCode.ARRAY_ADD_W_SIZE:
                        opStack.Push(FObject.NewArray(new FArray(NextInt())));
                        break;
                    case OpCode.ARRAY_ADD_BIG:
                        var initSize = opStack.Pop().Int();
                        var addSize = opStack.Pop().Int();
                        if (addSize < 0 || addSize > short.MaxValue)
                            throw new IndexOutOfRangeException();
                        opStack.Push(FObject.NewArray(
                            new FArray(initSize, (short)addSize)
                        ));
                        break;
                    case OpCode.ARRAY_GET:
                        var arr = opStack.Pop().Array();
                        indexObj = opStack.Pop();

                        opStack.Push(arr.Get(indexObj.I64));
                        break;
                    case OpCode.ARRAY_PUSH:
                        var val = opStack.Pop();
                        var ptr = opStack.Pop().PTR;
                        ((FArray)Heap.Get(ptr)).Push(val);
                        break;
                    case OpCode.ARRAY_SPLICE:
                        var to = opStack.Pop(); //
                        var from = opStack.Pop(); // both ints
                        arr = opStack.Pop().Array();
                        opStack.Push(FObject.NewArray(new FArray(arr, from.I64, to.I64)));
                        break;
                    case OpCode.ARRAY_SET:
                        arr = opStack.Pop().Array();
                        indexObj = opStack.Pop();
                        val = opStack.Pop();
                        arr.items[indexObj.I64] = val;
                        break;
                    // case OpCode.ARRAY_REMOVE:
                    //     indexObj = opStack.Pop(); // has to be int
                    //     var ptrObj = opStack.Pop();
                    //     arr = ptrObj.Array();
                    //     arr.Remove(indexObj.I64);
                    //     arr.pos--;
                    //     opStack.Push(ptrObj);
                    //     break;
                    // case OpCode.ARRAY_INSERT:
                    //     indexObj = opStack.Pop();
                    //     val = opStack.Pop();
                    //     arr = opStack.Pop().Array();
                    //     arr.Insert(indexObj.IsInt(), val);
                    //     opStack.Push(val);
                    //     break;
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

                if (pc % 10 == 0)
                    Garbage.ConditionalClean();
                previousOpCode = cCode;
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
                default:
                    throw new RuntimeException($"Import failed: package 'package' not found.");
            }
        }

        private OpCode NextOperation()
        {
            return (OpCode)immutableInstr[pc++];
        }

        /// <summary>
        /// Take 4 bytes from the instructions, move the pc by 4, and convert those bytes into an integer
        /// </summary>
        /// <returns></returns>
        private int NextInt()
        {
            return BitConverter.ToInt32(new[]
                {immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++]});
        }

        /// <summary>
        /// Take 4 bytes from the instructions, move the pc by 4, and convert those bytes into a float
        /// </summary>
        /// <returns></returns>
        private double NextDouble()
        {
            return BitConverter.ToDouble(new[]
                {
                    immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++],
                    immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++]
                });
        }
        private long NextLong()
        {
            return BitConverter.ToInt64(new[]
                {
                    immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++],
                    immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++], immutableInstr[pc++]
                });
        }

        /// <summary>
        /// Execute <c>Run()</c> on current VirtualMachine instance
        /// </summary>
        /// <returns>The operation stack topmost item, or &lt;nil&gt; if there are no items on the stack</returns>
        public FObject EvalRun()
        {
            Run();
            if (opStack.Count > 0)
                return opStack.Pop();
            return FObject.Nil();
        }

        public static class Garbage
        {
            public static long Threshold { get; set; } = 0xa00000; // 10mb
            private static readonly Queue<FObject> pending = new Queue<FObject>();
            private static long increase; // Increase in bytes since last clean() cycle.
            private static long old;

            public static void Mark(FObject obj)
            {
                pending.Enqueue(obj);
            }

            /// <summary>
            /// Run a clean cycle if the memory-usage increase threshold has been reached. Also updates the increase value.
            /// </summary>
            public static void ConditionalClean()
            {
                if (old == 0)
                    old = GC.GetTotalMemory(false);
                increase = GC.GetTotalMemory(false) - old;
                if (increase > Threshold)
                    Clean();
            }

            public static void Clean()
            {
                while (pending.Count > 0)
                {
                    var fobj = pending.Dequeue();
                    fobj.Dispose();
                }

                increase = 0;
                old = GC.GetTotalMemory(false);
            }
        }
    }
}