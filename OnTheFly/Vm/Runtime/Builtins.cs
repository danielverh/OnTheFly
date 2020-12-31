using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheFly.Vm.Runtime.Exceptions;

namespace OnTheFly.Vm.Runtime
{
    public static class Builtins
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exceptions.RuntimeException">Throws when the functions is not found.</exception>
        public static FBuiltin GetBuiltin(string name)
        {
            if (!functions.ContainsKey(name))
                throw new RuntimeException($"Functions with name '{name}' not found");
            return functions[name];
        }

        private static Dictionary<string, FBuiltin> functions = new Dictionary<string, FBuiltin>();

        static Builtins()
        {
            LoadArrayFunctions();
        }

        private static void LoadArrayFunctions()
        {
            functions["insert"] = new FBuiltin
            {
                // Arity count is not including caller object (array in this case)
                Arity = 2, CallObjectTypes = {FObjectType.Array},
                Invokable = (fi, ins) =>
                {
                    ins[0].Array().Insert(ins[1].I64, ins[2]);
                    return ins[0];
                },
                ArgumentTypes = {FObjectType.Array, FObjectType.Int, FObjectType.Any}
            };
            functions["count"] = new FBuiltin
            {
                Arity = 0, CallObjectTypes = {FObjectType.Array, FObjectType.String},
                Invokable = (fi, ins) => FObject.NewI64(ins[0].Array().Length),
            };
            functions["remove"] = new FBuiltin
            {
                Arity = 1, CallObjectTypes = {FObjectType.Array},
                Invokable = (fi, ins) =>
                {
                    ins[0].Array().Remove(ins[1].I64);
                    return ins[0];
                },
                ArgumentTypes = {FObjectType.Int}
            };
            functions["mutate"] = new FBuiltin
            {
                Arity = 1, CallObjectTypes = {FObjectType.Array},
                Invokable = (fi, ins) =>
                {
                    var oArr = ins[0].Array();
                    var func = ins[1].Function();
                    var nArr = new FArray(oArr.Length);
                    for (int i = 0; i < oArr.Length; i++)
                    {
                        nArr.Push(fi.Invoke(func, new[] {oArr.Get(i)}));
                    }
                    return FObject.NewArray(nArr);
                },
                ArgumentTypes = {FObjectType.Function}
            };
            functions["filter"] = new FBuiltin
            {
                Arity = 1,
                CallObjectTypes = { FObjectType.Array },
                Invokable = (fi, ins) =>
                {
                    var oArr = ins[0].Array();
                    var func = ins[1].Function();
                    var nArr = new FArray(oArr.Length);
                    for (int i = 0; i < oArr.Length; i++)
                    {
                        if(fi.Invoke(func, new[] { oArr.Get(i) }).True())
                            nArr.Push(oArr.Get(i));
                    }
                    return FObject.NewArray(nArr);
                },
                ArgumentTypes = { FObjectType.Function }
            };
        }
    }
}