using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using OnTheFly.Vm;
using OnTheFly.Vm.Helpers;
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace OnTheFly
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FObject
    {
        public static FObject NewI32(int i) => new FObject {Type = FObjectType.Int, I32 = i};
        public static FObject NewF32(float i) => new FObject {Type = FObjectType.Float, F32 = i};
        public static FObject NewBool(bool b) => new FObject {Type = FObjectType.Bool, BOOL = b};
        public static FObject NewString(int i) => new FObject {Type = FObjectType.String, PTR = i};

        public static FObject NewString(string str)
        {
            var o = new FObject {Type = FObjectType.String, PTR = VirtualMachine.Heap.Count};
            VirtualMachine.Heap.Add(str);
            return o;
        }
        public static FObject NewArray(FArray array)
        {
            var o = new FObject {Type = FObjectType.Array, PTR = VirtualMachine.Heap.Count};
            VirtualMachine.Heap.Add(array);
            return o;
        }
        public static FObject Nil() => new FObject {Type = FObjectType.Nil, BOOL = false};
        public bool True()
        {
            if (Type == FObjectType.Bool)
                return BOOL;
            if (Type == FObjectType.Nil)
                return false;
            return true;
        }

        public static FObject operator !(FObject r)
        {
            switch (r.Type)
            {
                case FObjectType.Bool:
                    return FObject.NewBool(!r.BOOL);
                case FObjectType.Nil:
                    return FObject.NewBool(true);
                case FObjectType.Float:
                case FObjectType.Int:
                case FObjectType.Array:
                case FObjectType.String:
                    return FObject.NewBool(!r.True());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static FObject operator +(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 + r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 + r.F32};
                case FObjectType.Array:
                    l.Array().Push(r);
                    return l;
                case FObjectType.String when r.Type == FObjectType.String:
                    var str = l.ToString() + r.ToString();
                    return NewString(str);
                case FObjectType.String:
                    return NewString(l.ToString() + r.ToString());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static FObject operator -(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 - r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 - r.F32};
                case FObjectType.String when r.Type == FObjectType.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static FObject operator *(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 * r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 * r.F32};
                case FObjectType.String when r.Type == FObjectType.Int:
                    return NewString(VirtualMachine.Heap[l.PTR].ToString().Repeat(r.I32));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static FObject operator /(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 / r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 / r.F32};
                case FObjectType.String when r.Type == FObjectType.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static FObject operator >(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return NewBool(l.I32 > r.I32);
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return NewBool(l.I32 > r.F32);
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return NewBool(l.F32 > r.I32);
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return NewBool(l.F32 > r.F32);
                default:
                    throw new InvalidOperationException();
            }
        }
        public static FObject operator <(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return NewBool(l.I32 < r.I32);
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return NewBool(l.I32 < r.F32);
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return NewBool(l.F32 < r.I32);
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return NewBool(l.F32 < r.F32);
                default:
                    throw new InvalidOperationException();
            }
        }
        public static FObject operator <= (FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return NewBool(l.I32 <= r.I32);
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return NewBool(l.I32 <= r.F32);
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return NewBool(l.F32 <= r.I32);
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return NewBool(l.F32 <= r.F32);
                default:
                    throw new InvalidOperationException();
            }
        }
        public static FObject operator >=(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return NewBool(l.I32 >= r.I32);
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return NewBool(l.I32 >= r.F32);
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return NewBool(l.F32 >= r.I32);
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return NewBool(l.F32 >= r.F32);
                default:
                    throw new InvalidOperationException();
            }
        }

        [FieldOffset(0)] public FObjectType Type;

        [FieldOffset(8)] public int I32;
        [FieldOffset(8)] public bool BOOL;
        [FieldOffset(8)] public float F32;
        [FieldOffset(8)] public int PTR;

        public override string ToString()
        {
            switch (Type)
            {
                case FObjectType.Int:
                    return I32.ToString();
                case FObjectType.Float:
                    return F32.ToString(CultureInfo.InvariantCulture);
                case FObjectType.Bool:
                    return BOOL ? "true" : "false";
                case FObjectType.Nil:
                    return "<nil>";
                case FObjectType.String:
                    return VirtualMachine.Heap[PTR].ToString();
                case FObjectType.Array:
                    var arr = VirtualMachine.Heap[PTR];
                    return arr.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Equals(object obj)
        {
            return Equals((FObject) obj);
        }

        public bool Equals(FObject o)
        {
            switch (Type)
            {
                case FObjectType.Int when o.Type == FObjectType.Int:
                    return o.I32 == I32;
                case FObjectType.Float when o.Type == FObjectType.Float:
                    return Math.Abs(o.F32 - F32) < 0.0001;
                case FObjectType.String when o.Type == FObjectType.String:
                    return o.ToString() == ToString();
                case FObjectType.Array when o.Type == FObjectType.Array:
                    return PTR == o.PTR || VirtualMachine.Heap[PTR] == VirtualMachine.Heap[o.PTR];
                case FObjectType.Bool when o.Type == FObjectType.Bool:
                    return o.BOOL == BOOL;
                default:
                    throw new ArgumentException();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode * 397) ^ I32;
                hashCode = (hashCode * 397) ^ BOOL.GetHashCode();
                hashCode = (hashCode * 397) ^ F32.GetHashCode();
                hashCode = (hashCode * 397) ^ PTR;
                return hashCode;
            }
        }

        public FArray Array()
        {
            if (Type == FObjectType.Array)
            {
                return (FArray) VirtualMachine.Heap[PTR];
            }
            else if (Type == FObjectType.String)
            {
                var str = VirtualMachine.Heap[PTR].ToString().ToCharArray();
                var l = new FObject[str.Length];
                for (var i = 0; i < str.Length; i++)
                {
                    l[i] = NewString(str[i].ToString());
                }

                return new FArray(l);
            }
            throw new Exception();
        }

        public string Raw()
        {
            switch (Type)
            {
                case FObjectType.String:
                    return "'" + ToString() + "'";
                default:
                    return ToString();
            }
        }

        public FObject Count()
        {
            switch (Type)
            {
                case FObjectType.Array:
                    return FObject.NewI32(Array().pos);
                case FObjectType.String:
                    return FObject.NewI32(VirtualMachine.Heap[PTR].ToString().Length);
                default:
                    throw new InvalidOperationException();
            }
        }

        public int Int()
        {
            if (Type == FObjectType.Int)
                return I32;
            throw new Exception($"{this} is not an integer.");
        }
    }

    public enum FObjectType
    {
        Nil,
        Int,
        Float,
        String,
        Array,
        Bool,
    }
}