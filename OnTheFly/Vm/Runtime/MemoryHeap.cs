using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Vm.Runtime
{
    /// <summary>
    /// Dictionary extension which handles unordened object storing in memory
    /// </summary>
    public class MemoryHeap
    {
        private readonly Dictionary<int, object> objects = new Dictionary<int, object>();

        /// <summary>
        /// Adds an object to the memory heap and returns its corresponding value
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int Add(object o)
        {
            var address = GetAddress();
            objects.Add(address, o);
            return address;
        }
        /// <summary>
        /// Returns an object corresponding to the address argument
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public object Get(int address) => objects[address];
        private int GetAddress()
        {
            if (objects.Count == 0) return 0;
            return objects.Last().Key + 1;
        }

        public void Dispose(int address)
        {
            objects.Remove(address);
        }
    }
}