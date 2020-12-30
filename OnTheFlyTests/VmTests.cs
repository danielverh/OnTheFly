using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using OnTheFly;
using OnTheFly.Vm;

namespace OnTheFlyTests
{
    public class VmTests
    {
        private TextWriter In;
        private TextReader Out;
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BinaryOperations()
        {
            (string, string)[] cases = 
            {
                ("25 + 35 - 20", "40"),
                ("25 + 35 * 2", "95"),
                ("25 / 2.0", "12.5"),
            };
            EvaluateList(cases);
        }
        [Test]
        public void CompareOperations()
        {
            (string, string)[] cases =
            {
                ("4 == 5", "false"),
                ("4 < 5", "true"),
                ("5 >= 5", "true"),
                ("6 >= 5", "true"),
                (@"""test"" == ""test""", "true"),
                (@"""test"" != ""a""", "true"),
                ("[1, 2] == [1, 2]", "true"),
                ("[1, 2] != [2, 1]", "true"),
            };
            EvaluateList(cases);
        }
        public void Expressions()
        {
            (string, string)[] cases =
            {
                ("my_int = 100; print(my_int);", "100"),
                ("my_float = 1.53; print(my_float); ", "1.53"),
                (@"my_string = ""Hello World!""; print(my_string);", "Hello World!"),
            };
            EvaluateList(cases);
        }
        [Test]
        public void Statements()
        {
            (string, string)[] cases =
            {
                ("i = 0; for i < 10 { i += 1; } i;", "10"),
                ("expr = true; if expr { a = expr; }", "true"),
                ("a = -1; if true { a = 0; } elif false { a = 1; } else { a = 2; } a;", "0"),
                ("a = -1; if false { a = 0; } elif true { a = 1; } else { a = 2; } a;", "1"),
                ("a = -1; if false { a = 0; } elif false { a = 1; } else { a = 2; } a;", "2"),
            };
            EvaluateList(cases);
        }
        public void EvaluateList((string, string)[] cases)
        {
            foreach (var (code, expected) in cases)
            {
                VirtualMachine.Heap.Clear();
                var parsed = FlyCode.Parse($"{code}");
                
                var result = FlyCode.RunEval(parsed.Instructions, parsed.Contexts);
                Assert.AreEqual(expected, result.ToString());
            }
        }
    }
}