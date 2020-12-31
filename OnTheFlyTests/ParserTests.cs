using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FlyLang;
using NUnit.Framework;
using OnTheFly;

namespace OnTheFlyTests
{
    public class ParserTests
    {
        [Test]
        public void TestStatements()
        {
            var cases = new[]
            {
                "test = 5;",
                "if true { print(0); } elif false { print(1); } else { print(2); }",
                "b = box (a) { return a; }",
                "a, b = 5, 4;",
            };
            foreach (var @case in cases)
            {
                var lexer = new FlyLexer(new AntlrInputStream(@case));

                var parser = new FlyParser(new CommonTokenStream(lexer));
                // parser.ErrorHandler= new BailErrorStrategy();
                var p = parser.program();
                Assert.True(p != null);
                
                // var listener = new Listener();
                // listener.EnterProgram(parser.program());
            }
        }
    }
}
