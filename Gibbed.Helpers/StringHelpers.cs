using System;
using System.Text;

namespace Gibbed.Helpers
{
    public static class StringHelpers
    {
        // FNV hash that EA loves to use :-)
        public static UInt32 HashFNV24(this string input)
        {
            UInt32 hash = input.HashFNV32();
            return (hash >> 24) ^ (hash & 0xFFFFFF);
        }

        public static UInt32 HashFNV32(this string input)
        {
            return input.HashFNV32(0x811C9DC5);
        }

        public static UInt32 HashFNV32(this string input, UInt32 hash)
        {
            string lower = input.ToLowerInvariant();

            for (int i = 0; i < lower.Length; i++)
            {
                hash *= 0x1000193;
                hash ^= (char)(lower[i]);
            }

            return hash;
        }

        public static UInt64 HashFNV64(this string input)
        {
            return input.HashFNV64(0xCBF29CE484222325);
        }

        public static UInt64 HashFNV64(this string input, UInt64 hash)
        {
            string lower = input.ToLowerInvariant();

            for (int i = 0; i < lower.Length; i++)
            {
                hash *= 0x00000100000001B3;
                hash ^= (char)(lower[i]);
            }

            return hash;
        }

        // From Prototype
        public static UInt64 Hash1003F(this string input)
        {
            UInt64 hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = (hash * 65599) ^ (char)(input[i]);
            }
            return hash;
        }

        // From Volition (SR2, RFG)
        public static UInt32 CrcVolition(this string input)
        {
            input = input.ToLowerInvariant();
            UInt32 hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = Tables.CrcVolition[(byte)hash ^ (byte)input[i]] ^ (hash >> 8);
            }
            return hash;
        }

        public static UInt32 HashVolition(this string input)
        {
            UInt32 hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                UInt32 c = (char)(input[i]);
                if ((c - 0x41) <= 0x19)
                {
                    c += 0x20;
                }

                // rotate left by 6
                hash = (hash << 6) | (hash >> (32 - 6));
                hash = c ^ hash;
            }
            return hash;
        }

        public static UInt32 ParseHex32(this string input)
        {
            if (input.StartsWith("0x"))
            {
                return UInt32.Parse(input.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return UInt32.Parse(input);
        }

        public static UInt64 ParseHex64(this string input)
        {
            if (input.StartsWith("0x"))
            {
                return UInt64.Parse(input.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return UInt64.Parse(input);
        }
    }
}
