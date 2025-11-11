using System;
using System.Text;
using System.Numerics;

namespace HeroServer
{
    public static class BaseHelper
    {
        // HexString

        static readonly String alpha16 = "0123456789abcdef";
        static readonly int[] valueHex = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        public static String BytesToBase16(byte[] bytes)
        {
            StringBuilder res16 = new StringBuilder(bytes.Length * 2);

            byte b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i];
                res16.Append(alpha16[b >> 4]);
                res16.Append(alpha16[b & 0xF]);
            }

            return res16.ToString();
        }

        public static byte[] Base16ToBytes(String str16)
        {
            byte[] bytes = new byte[str16.Length / 2];

            for (int x = 0, i = 0; i < str16.Length; i += 2, x++)
                bytes[x] = (byte)(valueHex[str16[i] - '0'] << 4 | valueHex[str16[i + 1] - '0']);

            return bytes;
        }

        public static String BigIntToBase16(BigInteger big)
        {
            return BytesToBase16(big.ToByteArray(true, true));
        }

        public static BigInteger Base16ToBigInt(String str16)
        {
            return new BigInteger(Base16ToBytes(str16), true, true);
        }

        // Base 62

        static readonly String alpha62 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static String ULongToBase62(ulong val)
        {
            StringBuilder res62 = new StringBuilder();
            ulong bLen = (ulong)alpha62.Length;

            for (; val > 0; val /= bLen)
                res62.Insert(0, alpha62[(int)(val % bLen)]);

            return res62.ToString();
        }

        public static ulong Base62ToULong(String str62)
        {
            ulong val = 0L;

            for (int i = 0; i < str62.Length; i++)
            {
                val *= 62UL;
                if (str62[i] <= '9')
                    val += str62[i] - 48UL; // -'0'
                else if (str62[i] <= 'Z')
                    val += str62[i] - 55UL; // -'A' + 10
                else
                    val += str62[i] - 61UL; // -'a' + 36
            }

            return val;
        }

        public static String BytesToBase62(byte[] bytes)
        {
            return BigIntToBase62(new BigInteger(bytes, true, true));
        }

        public static byte[] Base62ToBytes(String str62)
        {
            return Base62ToBigInt(str62).ToByteArray(true, true);
        }

        public static String BigIntToBase62(BigInteger big)
        {
            StringBuilder res62 = new StringBuilder();
            BigInteger bLen = alpha62.Length;

            for (; big > 0;)
            {
                big = BigInteger.DivRem(big, bLen, out BigInteger mod);
                res62.Insert(0, alpha62[(int)mod]);
            }

            return res62.ToString();
        }

        public static BigInteger Base62ToBigInt(String str62)
        {
            BigInteger big = 0;

            for (int i = 0; i < str62.Length; i++)
            {
                big *= 62;
                if (str62[i] <= '9')
                    big += str62[i] - 48; // -'0'
                else if (str62[i] <= 'Z')
                    big += str62[i] - 55; // -'A' + 10
                else
                    big += str62[i] - 61; // -'a' + 36
            }

            return big;
        }
    }
}