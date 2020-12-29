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
                ("25 / 2", "12.5"),
            };
            EvaluateList(cases);
        }
        public void Expressions()
        {
            (string, string)[] cases =
{
                ("my_int = 100;", "100"),
                ("my_float = 1.53;", "1.53"),
                (@"my_string = ""Hello World!"";", "Hello World!"),
            };
            EvaluateList(cases);
        }
        public void EvaluateList((string, string)[] cases)
        {
            foreach (var (code, expected) in cases)
            {
                VirtualMachine.Heap.Clear();
                var parsed = FlyCode.Parse($"{code};");
                var result = FlyCode.RunEval(parsed.Instructions, parsed.Contexts);
                Assert.AreEqual(result.ToString(), expected);
            }
        }
    }
}