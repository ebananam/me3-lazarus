using System.IO;
using System.Text;

namespace Gibbed.Helpers
{
	public static partial class StreamHelpers
	{
		public static string ReadStringASCII(this Stream stream, uint size, bool trailingNull)
		{
            return stream.ReadStringInternalStatic(Encoding.ASCII, size, trailingNull);
		}

        public static string ReadStringASCII(this Stream stream, uint size)
        {
            return stream.ReadStringInternalStatic(Encoding.ASCII, size, false);
        }

        public static string ReadStringASCIIZ(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.ASCII, '\0');
        }

        public static string ReadStringASCIINL(this Stream stream)
        {
            return stream.ReadStringInternalDynamic(Encoding.ASCII, '\n');
        }

		public static void WriteStringASCII(this Stream stream, string value)
		{
            stream.WriteStringInternalStatic(Encoding.ASCII, value);
		}

		public static void WriteStringASCIIZ(this Stream stream, string value)
		{
            stream.WriteStringInternalDynamic(Encoding.ASCII, value, '\0');
		}

        public static void WriteStringASCIINL(this Stream stream, string value)
        {
            stream.WriteStringInternalDynamic(Encoding.ASCII, value, '\n');
        }
	}
}
