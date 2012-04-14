using System.IO;
using System.Text;

namespace Gibbed.Helpers
{
    public static partial class StreamHelpers
    {
        public static string ReadStringUTF16(this Stream stream, uint size)
        {
            return stream.ReadStringInternalStatic(Encoding.Unicode, size, false);
        }

        public static string ReadStringUTF16(this Stream stream, uint size, bool trailingNull)
        {
            return stream.ReadStringInternalStatic(Encoding.Unicode, size, trailingNull);
        }

        public static string ReadStringUTF16(this Stream stream, bool littleEndian, uint size)
        {
            return stream.ReadStringInternalStatic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, size, false);
        }

        public static string ReadStringUTF16(this Stream stream, bool littleEndian, uint size, bool trailingNull)
        {
            return stream.ReadStringInternalStatic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, size, trailingNull);
        }

        public static string ReadStringUTF16Z(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.Unicode, '\0');
        }

        public static string ReadStringUTF16Z(this Stream stream, bool littleEndian)
        {
            return stream.ReadStringInternalDynamic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, '\0');
        }

        public static string ReadStringUTF16NL(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.Unicode, '\n');
        }

        public static string ReadStringUTF16NL(this Stream stream, bool littleEndian)
        {
            return stream.ReadStringInternalDynamic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, '\n');
        }

        public static void WriteStringUTF16(this Stream stream, string value)
        {
            stream.WriteStringInternalStatic(Encoding.Unicode, value);
        }

        public static void WriteStringUTF16(this Stream stream, bool littleEndian, string value)
        {
            stream.WriteStringInternalStatic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value);
        }

        public static void WriteStringUTF16Z(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(Encoding.Unicode, value, '\0');
        }

        public static void WriteStringUTF16Z(this Stream stream, bool littleEndian, string value)
        {
            stream.WriteStringInternalDynamic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value, '\0');
        }

        public static void WriteStringUTF16NL(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(Encoding.Unicode, value, '\n');
        }

        public static void WriteStringUTF16NL(this Stream stream, bool littleEndian, string value)
        {
            stream.WriteStringInternalDynamic(littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value, '\n');
        }
    }
}
