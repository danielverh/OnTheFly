using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm.Runtime
{
    /// <summary>
    /// This library maps functions to C# built-in Math functions
    /// </summary>
    public class MathLib : FLibrary
    {
        internal delegate float SingleFOp(float inp);

        public MathLib()
        {
            RegisterMethod("pi", 0, x => FObject.NewF32(MathF.PI));

            RegisterMethod("floor", 1, Single(MathF.Floor));
            RegisterMethod("ceiling", 1, Single(MathF.Ceiling));
            RegisterMethod("sin", 1, Single(MathF.Sin));
            RegisterMethod("cos", 1, Single(MathF.Cos));
            RegisterMethod("tan", 1, Single(MathF.Tan));
            RegisterMethod("asin", 1, Single(MathF.Asin));
            RegisterMethod("acos", 1, Single(MathF.Acos));
            RegisterMethod("atan", 1, Single(MathF.Atan));
            RegisterMethod("sqrt", 1, Single(MathF.Sqrt));
            RegisterMethod("log", 1, Single(MathF.Log));
            RegisterMethod("log10", 1, Single(MathF.Log10));

            RegisterMethod("logx", 2, x => FObject.NewF32(MathF.Log(x[0].IsFloat(), x[1].IsFloat())));
            RegisterMethod("power", 2, x => FObject.NewF32(MathF.Pow(x[0].IsFloat(), x[1].IsFloat())));
            RegisterMethod("round", 2, x => FObject.NewF32(MathF.Round(x[0].IsFloat(), x[1].IsInt())));
        }

        internal Method Single(SingleFOp op)
        {
            return x => FObject.NewF32(op.Invoke(x[0].IsFloat()));
        }
    }
}