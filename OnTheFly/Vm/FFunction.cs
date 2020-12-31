using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnTheFly.Helpers;
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

    public class FBuiltin : FFunction
    {
        public Func<FObject[], FObject> Invokable { get; set; }
        public FObjectType[] CallObjectTypes { get; set; }

        public FObject Invoke(FObject[] args)
        {
            var gotTypes = args.Select(x => x.Type).ToArray();
            var j = true;
            for (int i = 0; i < ArgumentTypes.Count; i++)
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
            return Invokable.Invoke(args);
        }
    }
}