using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using OnTheFly.Vm;
using OnTheFly.Vm.Runtime;

namespace OnTheFly
{
    public class VirtualMachine
    {
        private int[] instructions;

        private string[] constants;

        // TODO: Add garbage collector
        public static List<object> Heap = new List<object>();
        private Stack<FObject> opStack;
        private Stack<FCall> callStack;
        private Stack<FBlock> blockStack;
        private int pc;
        private Dictionary<string, FObject> globals = new Dictionary<string, FObject>();
        private Dictionary<string, FFunction> functions = new Dictionary<string, FFunction>(255);

        public VirtualMachine(Instructions _instructions)
        {
            instructions = _instructions.ToArray();
            constants = _instructions.StringConstants.ToArray();
            opStack = new Stack<FObject>(1024);
            callStack = new Stack<FCall>(128);
            blockStack = new Stack<FBlock>(256);
            blockStack.Push(new FBlock(globals));
            callStack.Push(new FCall(null, 0));
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public void Run()
        {
            while (pc < instructions.Length)
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
                        opStack.Push(blockStack.Peek()[constants[NextInt()]]);
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
                        var function = new FFunction() { Arity = NextInt(), Name = name };
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
                        blockStack.Pop().Close();
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
                        Console.Write(opStack.Pop());
                        break;
                    case OpCode.READ_LN:
                        opStack.Push(FObject.NewString(Console.ReadLine()));
                        break;
                    case OpCode.PRINT_LN:
                        Console.WriteLine();
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
                        ((FArray)Heap[ptr]).Push(val);
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
                        arr = opStack.Pop().Array();
                        arr.Remove(index.I32);
                        arr.pos--;
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
            return (OpCode)instructions[pc++];
        }

        private int NextInt()
        {
            return instructions[pc++];
        }

        private float NextFloat()
        {
            return BitConverter.Int32BitsToSingle(instructions[pc++]);
        }
    }
}