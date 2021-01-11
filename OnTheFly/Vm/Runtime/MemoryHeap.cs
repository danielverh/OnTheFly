using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Vm.Runtime
{
    /// <summary>
    /// Dictionary extension which handles unordened object storing in memory
    /// </summary>
    public class MemoryHeap
    {
        private static List<IntPtr> flyObjects = new List<IntPtr>();
        /// <summary>
        /// Adds an object to the memory heap and returns its corresponding value
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IntPtr Add(object o)
        {
            var ptr = (IntPtr)GCHandle.Alloc(o);
            flyObjects.Add(ptr);
            return ptr;
        }
        /// <summary>
        /// Returns an object corresponding to the address argument
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        #nullable enable
        public object? Get(IntPtr address) => GCHandle.FromIntPtr(address).Target;

        public void Dispose(IntPtr address)
        {
            GCHandle.FromIntPtr(address).Free();
        }

        public void Clear()
        {
            foreach (var ptr in flyObjects)
            {
                GCHandle.FromIntPtr(ptr).Free();
            }
            flyObjects.Clear();
        }
    }
}