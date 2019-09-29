using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm.Runtime
{
    public class FLibrary
    {
        public delegate FObject Method(FObject[] args);

        public Dictionary<string, Method> Methods = new Dictionary<string, Method>();
        public Dictionary<string, int> ArityMap = new Dictionary<string, int>();

        public void RegisterMethod(string name, int arity, Method method)
        {
            Methods[name] = method;
            ArityMap[name] = arity;
        }

        public FObject Invoke(string name, FObject[] args)
        {
            if (args.Length != ArityMap[name])
                throw new ArgumentOutOfRangeException($"Expected {ArityMap[name]} arguments; got {args.Length}");
            return Methods[name].Invoke(args);
        }

        public T Is<T>(FObject obj)
        {
            switch (obj.Type)
            {
                case FObjectType.Int when typeof(T) == typeof(int):
                    return (T) (object) obj.I32;
                case FObjectType.Float when typeof(T) == typeof(float):
                    return (T) (object) obj.F32;
                case FObjectType.String when typeof(T) == typeof(string):
                    return (T) (object) obj.ToString();
                case FObjectType.Array when typeof(T).IsArray:
                    return (T) (object) obj.Array();
                case FObjectType.Bool when typeof(T) == typeof(bool):
                    return (T) (object) true;
                default:
                    throw new ArgumentOutOfRangeException($"Expected {typeof(T).Name} got {obj.Type.ToString()}.");
            }
        }

        public bool Is(FObject obj, Type t)
        {
            switch (obj.Type)
            {
                case FObjectType.Int when t == typeof(int):
                case FObjectType.Float when t == typeof(float):
                case FObjectType.String when t == typeof(string):
                case FObjectType.Array when t.IsArray:
                case FObjectType.Bool when t == typeof(bool):
                    return true;
                default:
                    throw new ArgumentOutOfRangeException($"Expected {t.Name} got {obj.Type.ToString()}.");
            }
        }
    }
}