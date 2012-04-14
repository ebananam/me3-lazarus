using System.IO;
using System.Text;

namespace Gibbed.Helpers
{
    public static partial class StreamHelpers
    {
        public static string ReadStringUTF8(this Stream stream, uint size, bool trailingNull)
        {
            return stream.ReadStringInternalStatic(Encoding.UTF8, size, trailingNull);
        }

        public static string ReadStringUTF8(this Stream stream, uint size)
        {
            return stream.ReadStringInternalStatic(Encoding.UTF8, size, false);
        }

        public static string ReadStringUTF8Z(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.UTF8, '\0');
        }

        public static string ReadStringUTF8NL(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.UTF8, '\n');
        }

        public static void WriteStringUTF8(this Stream stream, string value)
        {
            stream.WriteStringInternalStatic(Encoding.UTF8, value);
        }

        public static void WriteStringUTF8Z(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(Encoding.UTF8, value, '\0');
        }

        public static void WriteStringUTF8NL(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(Encoding.UTF8, value, '\n');
        }
    }
}
