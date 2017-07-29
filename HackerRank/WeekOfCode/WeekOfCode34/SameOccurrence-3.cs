using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	private int n, q;
	private int[] arr;
	private Bucket[] buckets;
	private Bucket empty = new Bucket();
    private Query[] queries;
    
    void test()
    {
        const int period = 1000;
        n = 8000;
        q = 500000;
        arr = new int[n];
        for (int i=0; i<n; i++)
           arr[i] = (i % period)+1;
        queries = new Query[q];
        int fx = 1;
        int fy = 1;
        for (int i=0; i<q; i++)
        {
            queries[i] = new Query { X=fx, Y=fy };
            fy++;
            if (fy > period) {
                fy = 1;
                fx++;
                if (fx > period) { fx = 1; };
            }
        }
    }
    
    void remap()
    {
        var map = new Dictionary<int, int>();
        int id = 1;
        
        for(int i=0; i<n; i++)
        {
            var v = arr[i];
            if (!map.ContainsKey(v)) map[v] = id++;
            arr[i] = map[v];
        }

        buckets = new Bucket[id];
        for (int i=0; i<id; i++)
            buckets[i] = new Bucket { X=i };
   		for (int i=0; i<n; i++)
			buckets[arr[i]].Indices.Add(i);
        
        foreach(var qq in queries)
        {
            int v;
            qq.X = map.TryGetValue(qq.X, out v) ? v : 0;
            qq.Y = map.TryGetValue(qq.Y, out v) ? v : 0;
            if (qq.X > qq.Y)
                Swap(ref qq.X, ref qq.Y);            
        }
        
    }
    
	public void solve()
	{
		n = Ni();
		q = Ni();
		arr = Ni(n);

        queries = new Query[q];
        for (int i = 0; i < q; i++)
            queries[i] = new Query { X = Ni(), Y = Ni() };

        //test();
        remap();
        
        long allRanges = AllRanges(n);
		int time = 0;
		int freqStart = n;
		int[] freq = new int[2*n+1];
		int[] timestamp = new int[freq.Length];
        
        var answers = queries.ToArray();
        Array.Sort(queries, (a,b)=>
                   {
                       int cmp = a.X.CompareTo(b.X);
                       if (cmp != 0) return cmp;
                       return a.Y.CompareTo(b.Y);
                   });
        
        Query prevQuery = null;
		foreach (var qq in queries)
		{
			int x = qq.X;
			int y = qq.Y;
            
            if (prevQuery != null && prevQuery.X == x && prevQuery.Y == y)
            {
                qq.Answer = prevQuery.Answer;
                continue;
            }
            prevQuery= qq;

            if (x == y)
			{
				qq.Answer = allRanges;
				continue;
			}

			Bucket bx = buckets[x], by = buckets[y];

			int i = 0;
			int j = 0;
			int prev = 0;
			int bal = freqStart;
			time++;

			timestamp[bal] = time;
			freq[bal] = 1;

			long count = 0;
			while (i < bx.Indices.Count || j < by.Indices.Count)
			{
				int v;
				int newBal;
				if (i >= bx.Indices.Count || j < by.Indices.Count && by.Indices[j] < bx.Indices[i])
				{
					v = by.Indices[j++];
					newBal = bal-1;
				}
				else 
				{
					v = bx.Indices[i++];
					newBal = bal + 1;
				}

				if (timestamp[newBal] < time)
				{
					timestamp[newBal] = time;
					freq[newBal] = 0;
				}


				// Add the intermediate elements
				int gap = v - prev;
                if (gap > 0)
                {
                    count += gap * 1L * (gap - 1) >> 1;
                    count += gap * 1L * freq[bal];
                    freq[bal] += gap;
                }
                
				prev = v + 1;
				bal = newBal;

				count += freq[bal];
				freq[bal] += 1;
			}

			int gap2 = n - prev;
            count += gap2 * 1L * (gap2 - 1) >> 1;
            count += gap2 * 1L * freq[bal];
            
            qq.Answer = count;
		}
        
        foreach(var qq in answers)
            WriteLine(qq.Answer);
        
        Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
	}
    
	long AllRanges(long length)
	{
		return length * (length + 1) >> 1;
	}

    public class Query
    {
        public long Answer;
        public int X;
        public int Y;
    }

	public class Bucket
	{
		public int X;
		public List<int> Indices = new List<int>();
		public int First => Indices[0];
		public int Last => Indices[Indices.Count - 1];
	}
}


class CaideConstants {
    public const string InputFile = null;
    public const string OutputFile = null;
}

static partial class Library
{

	#region Common

	public static void Swap<T>(ref T a, ref T b)
	{
		var tmp = a;
		a = b;
		b = tmp;
	}

	public static void Clear<T>(T[] t, T value = default(T))
	{
		for (int i = 0; i < t.Length; i++)
			t[i] = value;
	}

	public static int BinarySearch<T>(T[] array, T value, int left, int right, bool upper = false)
		where T : IComparable<T>
	{
		while (left <= right)
		{
			int mid = left + (right - left) / 2;
			int cmp = value.CompareTo(array[mid]);
			if (cmp > 0 || cmp == 0 && upper)
				left = mid + 1;
			else
				right = mid - 1;
		}
		return left;
	}

	#endregion

	#region  Input
	static System.IO.Stream inputStream;
	static int inputIndex, bytesRead;
	static byte[] inputBuffer;
	static System.Text.StringBuilder builder;
	const int MonoBufferSize = 4096;

	public static void InitInput(System.IO.Stream input = null, int stringCapacity = 16)
	{
		builder = new System.Text.StringBuilder(stringCapacity);
		inputStream = input ?? Console.OpenStandardInput();
		inputIndex = bytesRead = 0;
		inputBuffer = new byte[MonoBufferSize];
	}

	static void ReadMore()
	{
		inputIndex = 0;
		bytesRead = inputStream.Read(inputBuffer, 0, inputBuffer.Length);
		if (bytesRead <= 0) inputBuffer[0] = 32;
	}

	public static int Read()
	{
		if (inputIndex >= bytesRead) ReadMore();
		return inputBuffer[inputIndex++];
	}

	public static T[] N<T>(int n, Func<T> func)
	{
		var list = new T[n];
		for (int i = 0; i < n; i++) list[i] = func();
		return list;
	}

	public static int[] Ni(int n)
	{
		var list = new int[n];
		for (int i = 0; i < n; i++) list[i] = Ni();
		return list;
	}

	public static long[] Nl(int n)
	{
		var list = new long[n];
		for (int i = 0; i < n; i++) list[i] = Nl();
		return list;
	}

	public static string[] Ns(int n)
	{
		var list = new string[n];
		for (int i = 0; i < n; i++) list[i] = Ns();
		return list;
	}

	public static int Ni()
	{
		var c = SkipSpaces();
		bool neg = c == '-';
		if (neg) { c = Read(); }

		int number = c - '0';
		while (true)
		{
			var d = Read() - '0';
			if ((uint)d > 9) break;
			number = number * 10 + d;
		}
		return neg ? -number : number;
	}

	public static long Nl()
	{
		var c = SkipSpaces();
		bool neg = c == '-';
		if (neg) { c = Read(); }

		long number = c - '0';
		while (true)
		{
			var d = Read() - '0';
			if ((uint)d > 9) break;
			number = number * 10 + d;
		}
		return neg ? -number : number;
	}

	public static char[] Nc(int n)
	{
		var list = new char[n];
		for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (char)c;
		return list;
	}

	public static byte[] Nb(int n)
	{
		var list = new byte[n];
		for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (byte)c;
		return list;
	}

	public static string Ns()
	{
		var c = SkipSpaces();
		builder.Clear();
		while (true)
		{
			if ((uint)c - 33 >= (127 - 33)) break;
			builder.Append((char)c);
			c = Read();
		}
		return builder.ToString();
	}

	public static int SkipSpaces()
	{
		int c;
		do c = Read(); while ((uint)c - 33 >= (127 - 33));
		return c;
	}
	#endregion

	#region Output

	static System.IO.Stream outputStream;
	static byte[] outputBuffer;
	static int outputIndex;

	public static void InitOutput(System.IO.Stream output = null)
	{
		outputStream = output ?? Console.OpenStandardOutput();
		outputIndex = 0;
		outputBuffer = new byte[65535];
		AppDomain.CurrentDomain.ProcessExit += delegate { Flush(); };
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

	public static void Write(long signedNumber)
	{
		ulong number = (ulong)signedNumber;
		if (signedNumber < 0)
		{
			Write('-');
			number = (ulong)(-signedNumber);
		}

		Reserve(20 + 1); // 20 digits + 1 extra
		int left = outputIndex;
		do
		{
			outputBuffer[outputIndex++] = (byte)('0' + number % 10);
			number /= 10;
		}
		while (number > 0);

		int right = outputIndex - 1;
		while (left < right)
		{
			byte tmp = outputBuffer[left];
			outputBuffer[left++] = outputBuffer[right];
			outputBuffer[right--] = tmp;
		}
	}

	public static void Write(object obj)
	{
		if (obj == null) return;

		var s = obj.ToString();
		Reserve(s.Length);
		for (int i = 0; i < s.Length; i++)
			outputBuffer[outputIndex++] = (byte)s[i];
	}

	public static void Write(char c)
	{
		Reserve(1);
		outputBuffer[outputIndex++] = (byte)c;
	}

	public static void Write(byte[] array, int count)
	{
		Reserve(count);
		Array.Copy(array, 0, outputBuffer, outputIndex, count);
		outputIndex += count;
	}

	static void Reserve(int n)
	{
		if (outputIndex + n <= outputBuffer.Length)
			return;

		Dump();
		if (n > outputBuffer.Length)
			Array.Resize(ref outputBuffer, Math.Max(outputBuffer.Length * 2, n));
	}

	static void Dump()
	{
		outputStream.Write(outputBuffer, 0, outputIndex);
		outputIndex = 0;
	}

	public static void Flush()
	{
		Dump();
		outputStream.Flush();
	}

	#endregion

}


public class Program
{
	public static void Main(string[] args)
	{
		InitInput(Console.OpenStandardInput());
		InitOutput(Console.OpenStandardOutput());
		Solution solution = new Solution();
		solution.solve();
		Flush();
#if DEBUG
		Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
	}
}
