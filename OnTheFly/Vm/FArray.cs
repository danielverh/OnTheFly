using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFly.Vm
{
    public class FArray
    {
        public FObject[] items;
        internal int pos;
        private int _add = 24;

        public FArray(int initSize, int addSize = 24)
        {
            items = new FObject[initSize];
            _add = addSize;
        }
        public FArray(FObject[] _items)
        {
            items = _items;
            pos = items.Length;
        }
        public FArray(FArray src, int fromInc, int toInc)
        {
            items = new FObject[toInc + 1 - fromInc];
            int j = 0;
            for (int i = fromInc; i <= toInc; i++)
            {
                items[j] = src.items[i];
                j++;
            }

            pos = items.Length;
        }
        public void Push(FObject obj)
        {
            if (pos >= items.Length)
                Extend(_add);
            items[pos++] = obj;
        }

        public FObject Get(int index)
        {
            if(index >= pos)
                throw new IndexOutOfRangeException();
            return items[index];
        }
        public void Insert(int index, FObject item)
        {
            index++;
            // for inclusive numbers
            if (items.Length <= pos - 1)
                Extend(_add);
            for (int i = index; i < pos; i++)
            {
                items[i + 1] = items[i];
            }
            items[index] = item;
            pos++;
        }
        public void Remove(int index)
        {
            items[index] = FObject.Nil();
            var n = new FObject[items.Length - 1];
            for (int i = 0; i < n.Length; i++)
            {
                if (i > index) 
                    n[i-1] = items[i];
                else
                {
                    n[i] = items[i];
                }
            }
            items = n;
        }

        private void Extend(int chunkSize)
        {
            var n = new FObject[items.Length + chunkSize];
            items.CopyTo(n, 0);
            items = n;
        }

        public override string ToString()
        {
            var c = items.Take(pos).Select(x=>x.Raw());
            return $"[{string.Join(", ", c)}]";
        }
        public FObject[] Get()
        {
            return items.Take(pos).ToArray();
        }
    }
}