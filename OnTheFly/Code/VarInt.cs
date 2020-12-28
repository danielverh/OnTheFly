using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Code
{
    /// <summary>
    /// Provides parsing and serialization for a variable sized int: a var int is either 1, 3, 5 or 9 bytes long.
    /// </summary>
    public class VarInt
    {
        public static byte[] Serialize(long value)
        {
            if (value <= 252 && value > 0)
                return new[] {(byte) value};
            if (value <= short.MaxValue && value >= short.MinValue)
                return new byte[] {253}.Concat(BitConverter.GetBytes((short) value)).ToArray();
            if (value <= int.MaxValue && value >= int.MinValue)
                return new byte[] {254}.Concat(BitConverter.GetBytes((int) value)).ToArray();
            return new byte[] {255}.Concat(BitConverter.GetBytes((long) value)).ToArray();
        }

        public static long Parse(byte[] data, int offset)
        {
            var first = data[offset++];
            if (first <= 252)
                return first;
            if (first == 253)
                return BitConverter.ToInt16(data, offset);
            if (first == 254)
                return BitConverter.ToInt32(data, offset);

            return BitConverter.ToInt64(data, offset);
        }
    }
}