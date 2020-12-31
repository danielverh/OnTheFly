using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFly.Vm
{
    public class FArray
    {
        public FObject[] items;
        private int pos;
        private short _add = 24;
        public int Length => pos;
        public FArray(long initSize, short addSize = 24)
        {
            items = new FObject[initSize];
            _add = addSize;
        }
        public FArray(FObject[] _items)
        {
            items = _items;
            pos = items.Length;
        }
        public FArray(FArray src, long fromInc, long toInc)
        {
            items = new FObject[toInc + 1 - fromInc];
            int j = 0;
            for (var i = fromInc; i <= toInc; i++)
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

        public FObject Get(long index)
        {
            if(index >= pos)
                throw new IndexOutOfRangeException();
            return items[index];
        }
        public void Insert(long index, FObject item)
        {
            if (items.Length <= pos - 1)
                Extend(_add);
            for (var i = index; i < pos; i++)
            {
                items[i + 1] = items[i];
            }
            items[index] = item;
            pos++;
        }
        public void Remove(long index)
        {
            items[index] = FObject.Nil;
            var n = new FObject[items.Length - 1];
            for (var i = 0; i < items.Length; i++)
            {
                if (i > index) 
                    n[i-1] = items[i];
                else if(i < n.Length)
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
            return items[..pos];
        }

        public override bool Equals(object? obj)
        {
            if (obj is FArray arr)
            {
                return Get().SequenceEqual(arr.Get());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(items.GetHashCode(), pos);
        }
    }
}