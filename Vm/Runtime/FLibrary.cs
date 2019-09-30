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
    }
    public static class FLibraryExtensions
    {
        public static float IsFloat(this FObject obj)
        {
            var res = obj.Is(ObjType.Number);
            if (res is int i)
                return i;
            else if (res is float f)
                return f;
            throw new Exception($"{res} is not a number");
        }
        public static int IsInt(this FObject obj)
        {
            var res = obj.Is(ObjType.Number);
            if (res is int i)
                return i;
            else if (res is float f)
                return (int)f;
            throw new Exception($"{res} is not a number");
        }
        public static FArray IsArray(this FObject obj)
        {
            return (FArray)Is(obj, ObjType.Array);
        }
        public static string IsString(this FObject obj)
        {
            return (string)Is(obj, ObjType.String);
        }
        public static bool IsBool(this FObject obj)
        {
            return (bool)Is(obj, ObjType.Bool);
        }
        public static object Is(this FObject obj, ObjType t)
        {

            switch (obj.Type)
            {
                case FObjectType.Int when t == ObjType.Number:
                    return obj.I32;
                case FObjectType.Float when t == ObjType.Number:
                    return obj.F32;
                case FObjectType.Array when t == ObjType.Array:
                    return obj.Array();
                case FObjectType.String when t == ObjType.String:
                    return obj.ToString();
                case FObjectType.Bool when t == ObjType.Bool:
                    return obj.BOOL;
                default:
                    throw new Exception($"Not able to cast {obj.Type} to a {t} ");
            }

        }
    }
    public enum ObjType
    {
        Number, String, Array, Bool
    }
}