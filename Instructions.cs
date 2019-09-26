using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly
{
    public class Instructions : List<int>
    {
        public List<string> StringConstants { get; set; } = new List<string>();
        public Instructions() : base()
        {
        }

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

        public void AddInt(string i)
        {
            Add(OpCode.LOAD_I32);
            base.Add(int.Parse(i));
        }

        public void AddFloat(string f)
        {
            Add(OpCode.LOAD_F32);
            base.Add(BitConverter.SingleToInt32Bits(float.Parse(f)));
        }

        public int AddString(string str, bool hasQuotes = false)
        {
            if (hasQuotes)
                str = str.Substring(1, str.Length - 2);
            StringConstants.Add(str);
            return StringConstants.Count - 1;
        }

        public void AddBool(string b)
        {
            base.Add(b.ToLower() == "true" ? 1 : 0);
        }
    }

    public enum OpCode
    {
        ADD, SUB, MUL, DIV,
        EQUALS,
        LOAD_I32, LOAD_F32, LOAD_STR, LOAD_BOOL, LOAD_NIL,

        JMP, JMP_EQ, JMP_NEQ,

        INVERSE, CLONE, POP,

        SET_VAR, GET_VAR, PRINT
    }
}