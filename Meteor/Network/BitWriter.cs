using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * File : BitWriter
 * Author : Filipe
 * Date : 13/02/2014 10:01:28
 * Description :
 *
 */

namespace Meteor.Network
{
    public static class BitWriter //TODO: Clean this up.
    {
        private static int RoundUp(int value, int multiple)
        {
            if (value % multiple != 0)
                return value + multiple - value % multiple;

            return value;
        }

        public static byte[] GetBytes(ICollection bits)
        {
            var bytes = new byte[Math.Max(1,bits.Count / 8)];
            bits.CopyTo(bytes, 0);
            
            //vérification si besoin d'avoir plus de 1 byte ou pas : Divine
            int i = 0;
            if (bytes.Length == 2)
                i = BitConverter.ToInt16(bytes, 0);
            else
                i = BitConverter.ToInt32(bytes, 0);
            if (i < 128 && i>1)
                return new byte[] { bytes[0] };
            else if (i < 32768)
                return new byte[] { bytes[0], bytes[1] };
            else if (i < 2147483648 )
                return new byte[] { bytes[0], bytes[1], bytes[2] };
            
            return bytes;
        }

        public static BitArray TrimZero(BitArray bits, bool round)
        {
            var size = bits.Count;

            for (; size > 0; size--)
            {
                if (bits[size - 1])
                    break;
            }

            var blah = size;
            if (blah == 0)
                blah = 16; //blah = 16; pas forcé d'avoir un short

            var newBits = new BitArray(round ? RoundUp(blah, 16) : blah);  //var newBits = new BitArray(round ? RoundUp(blah, 16) : blah);

            for (var i = 0; i < size; i++)
                newBits[i] = bits[i];

            return newBits;
        }

        public static BitArray AddFront(BitArray bits, int count)
        {
            var newBits = new BitArray(bits.Count + count);

            for (var i = 0; i < bits.Count; i++)
                newBits[i + count] = bits[i];

            return newBits;
        }

        public static BitArray AddBack(BitArray bits, int count)
        {
            var newBits = new BitArray(bits.Count + count);

            for (var i = 0; i < bits.Count; i++)
                newBits[i] = bits[i];

            return newBits;
        }
    }
}
