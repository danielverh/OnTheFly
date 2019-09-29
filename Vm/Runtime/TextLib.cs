using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFly.Vm.Runtime
{
    public class TextLib : FLibrary
    {
        internal delegate string SingleString(string input);

        internal delegate string DoubleString(string input, string arg2);

        internal delegate string TripleString(string input, string arg2, string arg3);

        public TextLib()
        {
            RegisterMethod("split", 2,
                args => FObject.NewArray(new FArray(
                    Is<string>(args[0]).Split(Is<string>(args[1]))
                        .Select(FObject.NewString).ToArray())));
            RegisterMethod("replace", 3, GetMethod((x, y, z) => x.Replace(y, z)));
        }

        internal Method GetMethod(SingleString str)
        {
            return x => FObject.NewString(str.Invoke(Is<string>(x[0])));
        }

        internal Method GetMethod(DoubleString str)
        {
            return x => FObject.NewString(str.Invoke(Is<string>(x[0]), Is<string>(x[1])));
        }

        internal Method GetMethod(TripleString str)
        {
            return x => FObject.NewString(str.Invoke(Is<string>(x[0]), Is<string>(x[1]), Is<string>(x[2])));
        }
    }
}