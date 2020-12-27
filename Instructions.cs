using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly
{
    public class Instructions : List<int>
    {
        public List<string> StringConstants { get; set; } = new List<string>();
        public Dictionary<string, int> Functions { get; set; } = new Dictionary<string, int>();
        public void Add(OpCode code)
        {
            base.Add((int) code);
        }

        public int Fillable()
        {
            Add(0);
            return Count - 1;
        }

        public void Fill(int pos, int val)
        {
            this[pos] = val;
        }

        public void LoadInt(string i)
        {
            Add(OpCode.LOAD_I32);
            base.Add(int.Parse(i));
        }

        public void LoadFloat(string f)
        {
            Add(OpCode.LOAD_F32);
            base.Add(BitConverter.SingleToInt32Bits(float.Parse(f)));
        }

        public int AddString(string str, bool hasQuotes = false)
        {
            if (hasQuotes)
                str = str[1..^1];
            if (StringConstants.Contains(str))
                return StringConstants.IndexOf(str);
            StringConstants.Add(str);
            return StringConstants.Count - 1;
        }

        public void LoadString(int str)
        {
            Add(OpCode.LOAD_STR);
            Add(str);
        }

        public void AddLoadString(string str, bool hasQuotes = false)
        {
            LoadString(AddString(str, hasQuotes));
        }

        public void AddBool(string b)
        {
            base.Add(b.ToLower() == "true" ? 1 : 0);
        }

        public void LoadBool(string b)
        {
            Add(OpCode.LOAD_BOOL);
            AddBool(b);
        }
        /// <summary>
        /// Gives a string representations with
        /// </summary>
        /// <returns></returns>
        public string Disassemble()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"INSTRUCTIONS DISASSEMBLED\n");
            var opCount = 1;
            for (var i = 0; i < Count; i++)
            {
                sb.Append($"{opCount}/{i}. ");
                var instr = (OpCode) this[i];
                switch (instr)
                {
                    case OpCode.ADD_I1:
                        sb.AppendLine("ADD 1");
                        break;
                    case OpCode.SUB_I1:
                        sb.AppendLine("SUBTRACT 1");
                        break;
                    case OpCode.LOAD_I32:
                        sb.AppendLine($"LOAD_I32 {this[++i]}");
                        break;
                    case OpCode.LOAD_F32:
                        sb.AppendLine($"LOAD_F32 {BitConverter.Int32BitsToSingle(this[++i])}");
                        break;
                    case OpCode.LOAD_STR:
                        sb.AppendLine($"LOAD_STR '{StringConstants[this[++i]]}'");
                        break;
                    case OpCode.LOAD_BOOL:
                        sb.AppendLine("LOAD_BOOL " + (this[++i] == 1 ? "'true'" : "'false'"));
                        break;
                    case OpCode.LOAD_NIL:
                        sb.AppendLine("LOAD_NIL");
                        break;

                    case OpCode.CALL_FUNCTION:
                        string name = StringConstants[this[++i]];
                        sb.AppendLine($"CALL_FUNCTION {name} with {Functions[name]} arguments");
                        break;
                    case OpCode.ADD_FUNCTION:
                        name = StringConstants[this[++i]];
                        sb.Append($"ADD_FUNCTION {name}(");
                        var argCount = Functions[name] = this[++i];
                        for (var j = 0; j < argCount; j++)
                        {
                            sb.Append(StringConstants[this[++i]] + ", ");
                        }

                        sb.AppendLine($") :{this[++i]}");
                        break;
                    case OpCode.JMP:
                        sb.AppendLine($"JMP: {this[++i]}");
                        break;
                    case OpCode.JMP_EQ:
                        sb.AppendLine($"JMP_WHEN_EQUAL:{this[++i]}");
                        break;
                    case OpCode.JMP_FALSE:
                        sb.AppendLine($"JMP_WHEN_NOT_EQUAL :{this[++i]}");
                        break;
                    case OpCode.UNINV:
                        sb.AppendLine("UNARY_INVERSE");
                        break;
                    case OpCode.CLONE:
                        sb.AppendLine("CLONE");
                        break;
                    case OpCode.POP:
                        sb.AppendLine("POP");
                        break;
                    case OpCode.SET_VAR:
                        sb.AppendLine($"SET_VAR {StringConstants[this[++i]]}");
                        break;
                    case OpCode.GET_VAR:
                        sb.AppendLine($"GET_VAR {StringConstants[this[++i]]}");
                        break;
                    case OpCode.PRINT:
                        sb.AppendLine("PRINT");
                        break;
                    case OpCode.PRINT_LN:
                        sb.AppendLine("PRINT_LINE");
                        break;
                    case OpCode.CALL_BUILTIN:
                        sb.AppendLine($"CALL_BUILTIN {StringConstants[this[++i]]} . {StringConstants[this[++i]]}");
                        break;
                    case OpCode.RETURN:
                        sb.AppendLine("RETURN");
                        break;
                    case OpCode.START_BLOCK:
                        sb.AppendLine("START_BLOCK");
                        break;
                    case OpCode.START_LOOP:
                        sb.AppendLine($"START_LOOP with end at:{this[++i]}");
                        break;
                    case OpCode.END_BLOCK:
                        sb.AppendLine("END_BLOCK");
                        break;
                    case OpCode.COUNT:
                        sb.AppendLine("COUNT");
                        break;
                    case OpCode.NO_OP:
                        sb.AppendLine("NO_OP");
                        break;
                    case OpCode.READ_LN:
                        sb.AppendLine("READ_LINE");
                        break;
                    case OpCode.ARRAY_ADD:
                        sb.AppendLine($"ARRAY_ADD");
                        break;
                    case OpCode.ARRAY_ADD_BIG:
                        sb.AppendLine($"ARRAY_ADD_BIG");
                        break;
                    case OpCode.ARRAY_GET:
                        sb.AppendLine("ARRAY_GET");
                        break;
                    case OpCode.ARRAY_PUSH:
                        sb.AppendLine("ARRAY_PUSH");
                        break;
                    case OpCode.ARRAY_REMOVE:
                        sb.AppendLine("ARRAY_REMOVE");
                        break;
                    case OpCode.ARRAY_SPLICE:
                        sb.AppendLine($"ARRAY_SPLICE");
                        break;
                    case OpCode.ARRAY_SET:
                        sb.AppendLine($"ARRAY_SET");
                        break;
                    case OpCode.ARRAY_INSERT:
                        sb.AppendLine($"ARRAY_INSERT");
                        break;
                    case OpCode.ARRAY_PUSH_LOT:
                        sb.AppendLine($"ARRAY_PUSH_LOT {this[++i]}");
                        break;
                    case OpCode.IMPORT:
                        sb.AppendLine($"IMPORT {StringConstants[this[++i]]}");
                        break;
                    case OpCode.BREAK:
                        sb.AppendLine("BREAK");
                        break;
                    default:
                        sb.AppendLine(instr.ToString());
                        break;
                }

                opCount++;
            }

            sb.AppendLine("\n----------\n");
            return sb.ToString();
        }

        public void StartBlock() => Add(OpCode.START_BLOCK);
        public void EndBlock() => Add(OpCode.END_BLOCK);
    }

    public enum OpCode
    {
        NO_OP,
        IMPORT,

        ADD,
        SUB,
        MUL,
        DIV,
        MOD,

        ADD_I1,
        SUB_I1,

        EQUALS,
        NOT_EQ,
        SMALLER,
        LARGER,
        SMALLER_EQ,
        LARGER_EQ,

        LOAD_I32,
        LOAD_F32,
        LOAD_STR,
        LOAD_BOOL,
        LOAD_NIL,

        ARRAY_ADD,
        ARRAY_ADD_BIG,
        ARRAY_PUSH,
        ARRAY_PUSH_LOT,
        ARRAY_REMOVE,
        ARRAY_GET,
        ARRAY_SPLICE,
        ARRAY_SET,
        ARRAY_INSERT,

        CALL_BUILTIN,
        CALL_FUNCTION,
        ADD_FUNCTION,
        ADD_ARGUMENT,
        RETURN,
        BREAK,
        START_BLOCK, END_BLOCK, START_LOOP,

        JMP,
        JMP_EQ,
        JMP_FALSE,

        UNINV,
        CLONE,
        POP,

        COUNT,

        SET_VAR,
        GET_VAR,
        PRINT,
        PRINT_LN,
        READ_LN,
    }
}