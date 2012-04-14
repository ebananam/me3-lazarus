using System;
using System.IO;
using System.Text;

namespace Gibbed.Helpers
{
    public static partial class StreamHelpers
    {
        public static string ReadStringPascal16(this Stream stream)
        {
            short length = stream.ReadValueS16();
            string text = stream.ReadStringASCII((uint)length);
            int padding = (-(length - 2)) & 3;
            stream.Seek(padding, SeekOrigin.Current);
            return text;
        }

        public static void WriteStringPascal16(this Stream stream, string value)
        {
            if (value.Length > 0xFFFF)
            {
                throw new ArgumentException("value is too long", "value");
            }

            short length = (short)value.Length;
            stream.WriteValueS16(length);
            stream.WriteStringASCII(value);
            byte[] padding = new byte[(-(length - 2)) & 3];
            stream.Write(padding, 0, padding.Length);
        }
    }
}
