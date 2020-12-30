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
    /// <summary>
    /// FObject is the dynamic object used in FlyLang
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FObject : IDisposable
    {
        public static FObject NewI32(int i) => new FObject {Type = FObjectType.Int, I32 = i};
        public static FObject NewF32(float i) => new FObject {Type = FObjectType.Float, F32 = i};
        public static FObject NewBool(bool b) => new FObject {Type = FObjectType.Bool, BOOL = b};
        public static FObject NewString(int i) => new FObject {Type = FObjectType.String, PTR = i};

        public static FObject NewString(string str)
        {
            var o = new FObject {Type = FObjectType.String, PTR = VirtualMachine.Heap.Add(str)};
            ;
            return o;
        }

        public static FObject NewArray(FArray array)
        {
            var o = new FObject {Type = FObjectType.Array, PTR = VirtualMachine.Heap.Add(array)};
            ;
            return o;
        }

        public static FObject Nil() => new FObject {Type = FObjectType.Nil, BOOL = false};

        /// <summary>
        /// Check if the object is <c>true</c>. If the object is of type <c>Bool</c>, return its corresponding value.
        /// Otherwise return true, except when the object is <c>nil</c>
        /// </summary>
        /// <returns></returns>
        public bool True()
        {
            if (Type == FObjectType.Bool)
                return BOOL;
            if (Type == FObjectType.Nil)
                return false;
            return true;
        }
        /// <summary>
        /// Negate the FObject
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Make the FObject negative
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static FObject operator -(FObject r)
        {
            switch (r.Type)
            {
                case FObjectType.Float:
                    return FObject.NewF32(-r.F32);
                case FObjectType.Int:
                    return FObject.NewI32(-r.I32);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Add two FObjects, using automatic type casting.
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
        public static FObject operator +(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 + r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 + r.F32};
                case FObjectType.Int when r.Type == FObjectType.Float:
                case FObjectType.Float when r.Type == FObjectType.Int:
                    l = l.Cast(FObjectType.Float);
                    r = r.Cast(FObjectType.Float);
                    return new FObject
                    {
                        Type = FObjectType.Float, F32 = l.F32 + r.F32
                    };
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
        /// <summary>
        /// Subtract two FObjects, using automatic type casting.
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
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
        /// <summary>
        /// Multiply two FObjects, using automatic type casting.
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
        public static FObject operator *(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 * r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 * r.F32};
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return new FObject { Type = FObjectType.Float, F32 = l.F32 * r.Cast(FObjectType.Float).F32 };
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.Cast(FObjectType.Float).F32 * r.F32};
                case FObjectType.String when r.Type == FObjectType.Int:
                    return NewString(VirtualMachine.Heap.Get(l.PTR).ToString().Repeat(r.I32));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Divide two FObjects, using automatic type casting.
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
        public static FObject operator /(FObject l, FObject r)
        {
            switch (l.Type)
            {
                case FObjectType.Int when r.Type == FObjectType.Int:
                    return new FObject {Type = FObjectType.Int, I32 = l.I32 / r.I32};
                case FObjectType.Float when r.Type == FObjectType.Float:
                    return new FObject {Type = FObjectType.Float, F32 = l.F32 / r.F32};
                case FObjectType.Float when r.Type == FObjectType.Int:
                    return new FObject
                        {Type = FObjectType.Float, F32 = l.Cast(FObjectType.Float).F32 / r.Cast(FObjectType.Float).F32};
                case FObjectType.Int when r.Type == FObjectType.Float:
                    return new FObject
                        {Type = FObjectType.Float, F32 = l.Cast(FObjectType.Float).F32 / r.Cast(FObjectType.Float).F32};
                case FObjectType.String when r.Type == FObjectType.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static FObject operator %(FObject l, FObject r)
        {
            if(l.Type != FObjectType.Int && r.Type != FObjectType.Int)
                throw new InvalidOperationException();
            return new FObject { Type = FObjectType.Int, I32 = l.I32 % r.I32 };
        }

        /// <summary>
        /// True if l is greater than r
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
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
        /// <summary>
        /// True if l is smaller than r
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
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
        /// <summary>
        /// True if l is smaller or equal compared to r
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
        public static FObject operator <=(FObject l, FObject r)
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
        /// <summary>
        /// True if l is greater or equal compared to r
        /// </summary>
        /// <param name="l">Left object in the expression</param>
        /// <param name="r">Right object in the expression</param>
        /// <returns></returns>
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
        /// <summary>
        /// Cast the current object to a given FObjectType.
        /// </summary>
        /// <param name="target">The target FObjectType</param>
        /// <returns>A new instance of the FObject with target as it FObjectType</returns>
        /// <exception cref="InvalidOperationException">Throw an exception if automatic type casting fails.</exception>
        public FObject Cast(FObjectType target)
        {
            if (Type == target)
                return this;
            switch (Type)
            {
                case FObjectType.Nil:
                    return new FObject {Type = target};
                case FObjectType.Int:
                    return target switch
                    {
                        FObjectType.Float => new FObject {Type = target, F32 = (float) I32},
                        _ => throw new InvalidOperationException()
                    };
                case FObjectType.Float:
                    return target switch
                    {
                        FObjectType.Int => new FObject {Type = target, I32 = (int) F32},
                        _ => throw new InvalidOperationException()
                    };
                case FObjectType.Bool:
                    return target switch
                    {
                        FObjectType.Float => new FObject {Type = target, F32 = BOOL ? 1 : 0},
                        FObjectType.Int => new FObject {Type = target, I32 = BOOL ? 1 : 0},
                        _ => throw new InvalidOperationException()
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [FieldOffset(0)] public FObjectType Type;

        [FieldOffset(4)] public int I32;
        [FieldOffset(4)] public bool BOOL;
        [FieldOffset(4)] public float F32;
        [FieldOffset(4)] public int PTR;

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
                    return VirtualMachine.Heap.Get(PTR).ToString();
                case FObjectType.Array:
                    var arr = VirtualMachine.Heap.Get(PTR);
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
                    return PTR == o.PTR || VirtualMachine.Heap.Get(PTR) == VirtualMachine.Heap.Get(o.PTR);
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
                return (FArray) VirtualMachine.Heap.Get(PTR);
            }
            else if (Type == FObjectType.String)
            {
                var str = VirtualMachine.Heap.Get(PTR).ToString().ToCharArray();
                var l = new FObject[str.Length];
                for (var i = 0; i < str.Length; i++)
                {
                    l[i] = NewString(str[i].ToString());
                }

                return new FArray(l);
            }

            throw new Exception("Subject is not an array.");
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
                    return FObject.NewI32(VirtualMachine.Heap.Get(PTR).ToString().Length);
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

        public void Dispose()
        {
            switch (Type)
            {
                case FObjectType.Nil:
                    break;
                case FObjectType.Int:
                    break;
                case FObjectType.Float:
                    break;
                case FObjectType.String:
                    break;
                case FObjectType.Array:
                    VirtualMachine.Heap.Dispose(PTR);
                    break;
                case FObjectType.Bool:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Flags]
    public enum FObjectType
    {
        Nil = 2,
        Int = 4,
        Float = 8,
        String = 16,
        Array = 32,
        Bool = 64,

        Any = 128,
    }
}