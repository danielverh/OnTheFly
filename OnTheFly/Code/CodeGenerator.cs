using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Code
{
    [Serializable]
    public class CodeGenerator
    {
        public Instructions Instructions = new Instructions();
        public CodeContexts Contexts = new CodeContexts();
        /// <summary>
        /// Break a loop.
        /// </summary>
        public void Break()
        {
            Instructions.Add(OpCode.BREAK);
        }
        /// <summary>
        /// Use an int literal.
        /// </summary>
        public void Int(string content)
        {
            var l = long.Parse(content, CultureInfo.InvariantCulture);
            if (l > int.MaxValue || l < int.MinValue)
            {
                Instructions.Add(OpCode.LOAD_I64);
                Instructions.AddLong(l);
                return;
            }
            Instructions.LoadInt(content);
        }
        /// <summary>
        /// Use an int literal.
        /// </summary>
        public void Float(string content)
        {
            Instructions.LoadFloat(content);
        }
        /// <summary>
        /// Use a nil value.
        /// </summary>
        public void Nil()
        {
            Instructions.Add(OpCode.LOAD_NIL);
        }
        /// <summary>
        /// Use a string literal
        /// </summary>
        /// <param name="value"></param>
        public void String(string value)
        {
            Instructions.Add(OpCode.LOAD_STR);
            var stringIndex = Instructions.AddString(value, true);
            Instructions.AddInt(stringIndex);
        }

        public void Bool(string value)
        {
            Instructions.LoadBool(value);
        }

        public void VarCall(string name)
        {
            var i = Instructions.AddString(name);
            Instructions.Add(OpCode.GET_VAR);
            Instructions.AddInt(i);
        }

        public void ArrayGet()
        {
            Instructions.Add(OpCode.ARRAY_GET);
        }

        public void LibFunctionCall(string lib, string methodCallName)
        {
            Instructions.Add(OpCode.CALL_LIBFUNC);
            Instructions.AddInt(Instructions.AddString(lib));
            Instructions.AddInt(Instructions.AddString(methodCallName));
        }

        public void UnaryInversion()
        {
            Instructions.Add(OpCode.UNINV);
        }
        public void BinaryOperator(string op)
        {
            switch (op)
            {
                case "+":
                    Instructions.Add(OpCode.ADD);
                    break;
                case "-":
                    Instructions.Add(OpCode.SUB);
                    break;
                case "*":
                    Instructions.Add(OpCode.MUL);
                    break;
                case "/":
                    Instructions.Add(OpCode.DIV);
                    break;
                case "%":
                    Instructions.Add(OpCode.MOD);
                    break;
                default:
                    throw new ArgumentException($"Invalid operator {op}.");
            }
        }

        public void CompareOperator(string op)
        {
            switch (op)
            {
                case "==":
                    Instructions.Add(OpCode.EQUALS);
                    break;
                case "!=":
                    Instructions.Add(OpCode.NOT_EQ);
                    break;
                case "<":
                    Instructions.Add(OpCode.SMALLER);
                    break;
                case ">":
                    Instructions.Add(OpCode.LARGER);
                    break;
                case ">=":
                    Instructions.Add(OpCode.LARGER_EQ);
                    break;
                case "<=":
                    Instructions.Add(OpCode.SMALLER_EQ);
                    break;
            }
        }

        public void GetVar(string name)
        {
            Instructions.Add(OpCode.GET_VAR);
            Instructions.AddInt(Instructions.AddString(name));
        }

        public void SetVar(string name)
        {
            Instructions.Add(OpCode.SET_VAR);
            Instructions.AddInt(Instructions.AddString(name));
        }

        internal void UnaryOperator(string text)
        {
            if(text == "!")
                UnaryInversion();
            else
                Instructions.Add(OpCode.UN_NEGATIVE);
        }

        public void MethodDefinition(string name, string[] args, Action bodyBlock)
        {
            Instructions.Add(OpCode.ADD_FUNCTION);
            Instructions.AddInt(Instructions.AddString(name));
            Instructions.AddInt(args.Length);
            foreach (var arg in args)
            {
                Instructions.AddInt(Instructions.AddString(arg));
            }

            var endPos = Instructions.FillableInt();

            Instructions.Block(bodyBlock);

            Instructions.FillInt(endPos, Instructions.Count);
        }

        public void AnonymousMethodDefinitions(string[] args, Action bodyBlock)
        {
            Instructions.Add(OpCode.ADD_AN_FUNCTION);
            Instructions.AddInt(args.Length);
            foreach (var arg in args)
            {
                Instructions.AddInt(Instructions.AddString(arg));
            }

            var endPos = Instructions.FillableInt();

            Instructions.Block(bodyBlock);

            Instructions.FillInt(endPos, Instructions.Count);
        }
        public void Builtin(string name)
        {
            Instructions.Add(OpCode.CALL_BUILTIN);
            Instructions.AddInt(Instructions.AddString(name));
        }

        public void FunctionCall(string name)
        {

            Instructions.Add(OpCode.CALL_FUNCTION);
            Instructions.AddInt(Instructions.AddString(name));
        }

        public void Return()
        {
            Instructions.Add(OpCode.RETURN);
        }

        public void Import(string lib)
        {
            Instructions.Add(OpCode.IMPORT);
            Instructions.AddInt(Instructions.AddString(lib));
        }

        public void ArraySet()
        {
            Instructions.Add(OpCode.ARRAY_SET);
        }
    }
}
