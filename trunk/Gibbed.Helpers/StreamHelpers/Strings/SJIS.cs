using System.IO;
using System.Text;

namespace Gibbed.Helpers
{
    public static partial class StreamHelpers
    {
        private static Encoding SJIS = Encoding.GetEncoding(932);

        public static string ReadStringSJIS(this Stream stream, uint size, bool trailingNull)
        {
            return stream.ReadStringInternalStatic(SJIS, size, trailingNull);
        }

        public static string ReadStringSJIS(this Stream stream, uint size)
        {
            return stream.ReadStringInternalStatic(SJIS, size, false);
        }

        public static string ReadStringSJISZ(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(SJIS, '\0');
        }

        public static string ReadStringSJISNL(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(SJIS, '\n');
        }

        public static void WriteStringSJIS(this Stream stream, string value)
        {
            stream.WriteStringInternalStatic(SJIS, value);
        }

        public static void WriteStringSJISZ(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(SJIS, value, '\0');
        }

        public static void WriteStringSJISNL(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(SJIS, value, '\n');
        }
    }
}
