using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheFly.Vm.Runtime
{
    public class MathLib : FLibrary
    {
        internal delegate float SingleFOp(float inp);

        public MathLib()
        {
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
        }

        internal Method Single(SingleFOp op)
        {
            return x =>
            {
                ;
                return FObject.NewF32(op.Invoke(Is<float>(x[0])));
            };
        }
    }
}