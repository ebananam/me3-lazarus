using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gibbed.Helpers
{
	public static partial class ByteHelpers
	{
        /// <summary>
        /// Set the contents of a byte array to the specified value.
        /// </summary>
        public static void Reset(this byte[] data, byte value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = value;
            }
        }

        public static T ToStructure<T>(this byte[] data, int index)
		{
			int size = Marshal.SizeOf(typeof(T));

			if (index + size > data.Length)
			{
				throw new Exception("not enough data to fit the structure");
			}

			byte[] buffer = new byte[size];
			Array.Copy(data, index, buffer, 0, size);

			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			T structure = (T)(Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)));
			handle.Free();
			return structure;
		}

		public static T ToStructure<T>(this byte[] data)
		{
			return data.ToStructure<T>(0);
		}

		public static string ToStringASCIIZ(this byte[] data, int offset)
		{
			int i = offset;

			while (i < data.Length)
			{
				if (data[i] == 0)
				{
					break;
				}

				i++;
			}

			if (i == offset)
			{
				return "";
			}

			return Encoding.ASCII.GetString(data, offset, i - offset);
		}

		public static string ToStringASCIIZ(this byte[] data, uint offset)
		{
			return data.ToStringASCIIZ((int)offset);
		}

		public static string ToStringUTF8Z(this byte[] data, int offset)
		{
			int i = offset;

			while (i < data.Length)
			{
				if (data[i] == 0)
				{
					break;
				}

				i++;
			}

			if (i == offset)
			{
				return "";
			}

			return Encoding.UTF8.GetString(data, offset, i - offset);
		}

		public static string ToStringUTF8Z(this byte[] data, uint offset)
		{
			return data.ToStringUTF8Z((int)offset);
		}

		public static string ToStringUTF16Z(this byte[] data, int offset)
		{
			int i = offset;

			while (i < data.Length)
			{
				if (BitConverter.ToUInt16(data, i) == 0)
				{
					break;
				}

				i += 2;
			}

			if (i == offset)
			{
				return "";
			}

			return Encoding.Unicode.GetString(data, offset, i - offset);
		}

		public static string ToStringUTF16Z(this byte[] data, uint offset)
		{
			return data.ToStringUTF16Z((int)offset);
		}
	}
}
