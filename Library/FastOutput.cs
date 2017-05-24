using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.Library
{
	public static class FastOutput
	{
		static Stream outputStream = Console.OpenStandardOutput();
		static byte[] scratch = new byte[20];
		static byte[] buffer = new byte[65535];
		static int index;

		public static void Write(object obj)
		{
			if (obj == null) return;
			var s = obj.ToString();

			if (index + s.Length > buffer.Length)
				Dump();

			if (buffer.Length < s.Length)
				Array.Resize(ref buffer, Math.Max(buffer.Length * 2, s.Length));

			for (int i = 0; i < s.Length; i++)
				buffer[index++] = (byte)s[i];
		}

		public static void Write(char c)
		{
			if (index + 1 > buffer.Length)
				Dump();
			buffer[index++] = (byte)c;
		}

		public static void WriteLine(object obj = null)
		{
			Write(obj);
			Write('\n');
		}

		public static void WriteLine(long number)
		{
			Write(number);
			Write('\n');
		}

		public static void Write(long number)
		{
			int count = 0;

			if (number < 0)
			{
				scratch[count++] = (byte)'-';
				number = -number;
			}

			int start = count;
			while (number > 0 || count == start)
			{
				scratch[count++] = (byte)('0' + number % 10);
				number /= 10;
			}

			int left = start;
			int right = count - 1;
			while (left < right)
			{
				byte tmp = scratch[left];
				scratch[left++] = scratch[right];
				scratch[right--] = tmp;
			}

			Write(scratch, count);
		}

		public static void Write(byte[] array, int count)
		{
			if (index + count + 1 > buffer.Length)
				Dump();

			for (int i = 0; i < count; i++)
				buffer[index++] = array[i];
		}
		
		static void Dump()
		{
			outputStream.Write(buffer, 0, index);
			index = 0;
		}

		public static void Flush()
		{
			Dump();
			outputStream.Flush();
		}
	}
}
