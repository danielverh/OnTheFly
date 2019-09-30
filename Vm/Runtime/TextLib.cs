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
                    args[0].IsString().Split(args[1].IsString())
                        .Select(FObject.NewString).ToArray())));
            RegisterMethod("replace", 3, GetMethod((x, y, z) => x.Replace(y, z)));
            RegisterMethod("join", 2, (x)=>
            {
                var arr = x[0];
                if (arr.Type != FObjectType.Array)
                    throw new Exception();
                var glue = x[1].IsString();
                return FObject.NewString(string.Join(glue, arr.Array().Get().Select(y => y.ToString())));
            });
        }

        internal Method GetMethod(SingleString str)
        {
            return x => FObject.NewString(str.Invoke(x[0].IsString()));
        }

        internal Method GetMethod(DoubleString str)
        {
            return x => FObject.NewString(str.Invoke(x[0].IsString(), x[1].IsString()));
        }

        internal Method GetMethod(TripleString str)
        {
            return x => FObject.NewString(str.Invoke(x[0].IsString(), x[1].IsString(), x[2].IsString()));
        }
    }
}