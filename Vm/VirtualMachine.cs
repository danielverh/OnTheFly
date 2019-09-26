using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace OnTheFly
{
    public class VirtualMachine
    {
        private int[] instructions;
        private string[] constants;
        public static List<string> Heap = new List<string>();
        private Stack<FObject> opStack;
        private int pc;
        private bool comperator = false;
        private Dictionary<string, FObject> variables = new Dictionary<string, FObject>();

        public VirtualMachine(Instructions _instructions)
        {
            instructions = _instructions.ToArray();
            constants = _instructions.StringConstants.ToArray();
            this.opStack = new Stack<FObject>();
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
                        var i = BitConverter.Int32BitsToSingle(NextInt());
                        opStack.Push(FObject.NewF32(i));
                        break;
                    case OpCode.LOAD_STR:
                        Heap.Add(constants[NextInt()]);
                        opStack.Push(FObject.NewString(Heap.Count - 1));
                        break;
                    case OpCode.LOAD_NIL:
                        opStack.Push(FObject.Nil());
                        break;
                    case OpCode.SET_VAR:
                        variables[constants[NextInt()]] = opStack.Pop();
                        break;
                    case OpCode.GET_VAR:
                        opStack.Push(variables[constants[NextInt()]]);
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
                    case OpCode.INVERSE:
                        opStack.Push(!opStack.Pop());
                        break;
                    case OpCode.EQUALS:
                        comperator = opStack.Pop().Equals(opStack.Pop());
                        break;
                    case OpCode.JMP:
                        pc = NextInt();
                        break;
                    case OpCode.JMP_EQ:
                        if (opStack.Pop().True())
                            pc = NextInt();
                        else
                            pc++;
                        break;
                    case OpCode.JMP_NEQ:
                        if (!comperator)
                            pc = NextInt();
                        else
                            pc++;
                        break;
                    case OpCode.CLONE:
                        opStack.Push(opStack.Peek());
                        break;
                    case OpCode.PRINT:
                        Console.WriteLine(opStack.Pop());
                        break;
                }
            }
        }

        private OpCode NextOperation()
        {
            return (OpCode) instructions[pc++];
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