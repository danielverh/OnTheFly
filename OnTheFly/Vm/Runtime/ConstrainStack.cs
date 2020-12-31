using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Vm.Runtime
{
    public class ConstrainStack<T> : Stack<T>
    {
        public int Maximum { get; }

        public ConstrainStack(int maximum) : base()
        {
            Maximum = maximum;
        }
        public new void Push(T item)
        {
            if(Count >= Maximum)
                throw new StackOverflowException();
            base.Push(item);
        }
    }
}
