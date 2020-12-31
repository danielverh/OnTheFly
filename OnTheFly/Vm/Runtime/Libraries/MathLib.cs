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
        internal delegate double DoubleOp(double inp);

        public MathLib()
        {
            RegisterMethod("pi", 0, x => FObject.NewF64(Math.PI));

            RegisterMethod("floor", 1, Double(Math.Floor));
            RegisterMethod("ceiling", 1, Double(Math.Ceiling));
            RegisterMethod("sin", 1, Double(Math.Sin));
            RegisterMethod("cos", 1, Double(Math.Cos));
            RegisterMethod("tan", 1, Double(Math.Tan));
            RegisterMethod("asin", 1, Double(Math.Asin));
            RegisterMethod("acos", 1, Double(Math.Acos));
            RegisterMethod("atan", 1, Double(Math.Atan));
            RegisterMethod("sqrt", 1, Double(Math.Sqrt));
            RegisterMethod("log", 1, Double(Math.Log));
            RegisterMethod("log10", 1, Double(Math.Log10));

            RegisterMethod("logx", 2, x => FObject.NewF64(Math.Log(x[0].IsFloat(), x[1].IsFloat())));
            RegisterMethod("power", 2, x => FObject.NewF64(Math.Pow(x[0].IsFloat(), x[1].IsFloat())));
            RegisterMethod("round", 2, x => FObject.NewF64(Math.Round(x[0].IsFloat(), x[1].IsInt())));
        }

        internal Method Double(DoubleOp op)
        {
            return x => FObject.NewF64(op.Invoke(x[0].IsFloat()));
        }
    }
}