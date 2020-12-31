using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnTheFly.Helpers;
using OnTheFly.Vm.Runtime;
using OnTheFly.Vm.Runtime.Exceptions;

namespace OnTheFly.Vm
{
    public class FFunction
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Arity { get; set; }
        public string Name { get; set; }
        public List<string> Arguments { get; set; } = new List<string>();
        public List<FObjectType> ArgumentTypes { get; set; } = new List<FObjectType>();
    }

    public class FAnonymous : FFunction
    {
    }
    public class FBuiltin : FFunction
    {
        public Func<FunctionInvoker, FObject[], FObject> Invokable { get; set; }
        public List<FObjectType> CallObjectTypes { get; set; } = new List<FObjectType>();

        public FObject Invoke(FunctionInvoker fi, FObject[] args)
        {
            var gotTypes = args.Select(x => x.Type).ToArray();
            bool j = CallObjectTypes.Contains(gotTypes[0]);
            for (int i = 1; j && i < ArgumentTypes.Count; i++)
            {
                var item = ArgumentTypes[i];
                if (item != FObjectType.Any && item != gotTypes[i])
                {
                    j = false;
                    break;
                }
            }
            
            if (!j)
                throw new RuntimeException($"Call did not match function signature of function '{Name}'. " +
                                           $"Expected {ArgumentTypes.PrettyArray()}, got {gotTypes.PrettyArray()}.");
            return Invokable.Invoke(fi, args);
        }
    }
}