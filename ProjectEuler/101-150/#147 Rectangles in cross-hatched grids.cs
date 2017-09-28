using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
    static long inv4 = Inverse(4);
    static long inv6 = Inverse(6);
    static long inv24 = Inverse(24);
    
	public void solve()
	{
		int t = Ni();

		var len = 1001;
		/*long[,,] s = new long[len, len, 2];

		for (long m = 1; m < len; m++)
		{
			for (long n = 1; n <= m; n++)
			{
				long sq = m * (m + 1) % MOD * n % MOD * (n + 1) % MOD * inv4 % MOD;
				long sq2 = (2 * m - n) % MOD * (n * n % MOD * 4 - 1) % MOD - 3;
				sq2 = Fix(sq2 * n % MOD * inv6);
				s[m, n, 0] = sq;
				s[m, n, 1] = sq2;
				s[n, m, 0] = sq;
				s[n, m, 1] = sq2;
			}
		}

		for (int i = 1; i < len; i++)
		{
			for (int j = 1; j < len; j++)
			{
				s[i, j, 0] = (s[i,j, 0] + s[i,j-1, 0]) % MOD;
				s[i, j, 1] = (s[i,j, 1] + s[i,j-1, 1]) % MOD;
			}
		}

		for (int i = 1; i < len; i++)
		{
			for (int j = 1; j < len; j++)
			{
				s[i, j, 0] = (s[i, j, 0] + s[i-1, j, 0]) % MOD;
				s[i, j, 1] = (s[i, j, 1] + s[i-1, j, 1]) % MOD;
			}
		}*/

        /*
        Console.Error.WriteLine("I:" + SumIFrom01ToN(4));
        Console.Error.WriteLine("I2:" + SumI2From01ToN(4));
        Console.Error.WriteLine("I3:" + SumI3From01ToN(4));
        Console.Error.WriteLine("I4:" + SumI4From01ToN(4));
        */
        
		while (t-- > 0)
		{
			long m = Nl(), n = Nl();
			if (m < n) Swap(ref m, ref n);
            
            var term = inv4 * X(m) % MOD * X(n) % MOD;
            //var term2a = Mult(-4, SumI4From01ToN(n)) + SumI2From01ToN(n) + Mult(-3, SumIFrom01ToN(n));
            //var term2b = Mult(4, SumI3From01ToN(n)) - SumIFrom01ToN(n);
            //var term2 = Mult(2,Mult(SumIFrom01ToN(m), Fix(term2b))) + Mult(m, Fix(term2a));
            //term2 = Fix(term2) * inv6 % MOD;
            
            var a = n;
            var b = m;
            
            // Up to Min(n,m) -- Multiply by 2
            var term2x = Mult(a, a+1);
            var term2y = a-1;
            var term2a = term2y*Poly(a,2,2,-3,-8)%MOD;
            // (a,a)
            var term2b = term2y*Poly(a,4,10,9)%MOD;
            var term2c = (b - a)%MOD*(Poly(a,2,8,7,-17)%MOD + b*Poly(a, 10, 10, -5)%MOD)%MOD;                
                
            var term2 = ((term2a + term2b)%MOD + Div(term2c, 2))%MOD;
			term2 = Mult(term2x, Div(term2, 30));
            
            WriteLine($"{Fix(term)} {Fix(term2)} ");
			//Console.Error.WriteLine($"Interpolation: {Fix(term)} {Fix(term2)}");
            //Console.Error.WriteLine($"Correct: {Fix(s[m,n, 0])} {Fix(s[m,n, 1])}");
            
		}
	}
    
    public long Poly(long n, params long[] array)
    {
        long result = 0;
        foreach(var v in array)
            result = (n*result%MOD + v) % MOD;
        return result;
    }
    
    public long X(long n)
    {
        return n * (n+1) % MOD * (2*n %MOD +4) % MOD * inv6 % MOD;
    }

    public static long SumIFrom01ToN(long n)
    {
        return Div(Mult(n, n + 1), 2);
    }

    public static long SumI2From01ToN(long n)
    {
        // Square Pyramidal Number
        // Alternative, n * (n + 1) * (2 * n + 1) / 6;
        // Alternative, n*n*n/3 + n*n/2 + n/6
        return Mult(SumIFrom01ToN(n), Div(2 * n + 1, 3));
    }

    public static long SumI3From01ToN(long n)
    {
        // Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4
        var tmp = SumIFrom01ToN(n);
        return Mult(tmp, tmp);
    }

    public static long SumI4From01ToN(long n)
    {
        // Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4 (real numbers)
        // Alternative, ((((6 * n + 15) * n + 10) * n) * n - 1) * n / 30;
        return Mult(SumI2From01ToN(n), Div(Mult(3 * n, n + 1) - 1,5));

    }    


	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[3000];

		if (n < _inverse.Length && (result = _inverse[n]) != 0)
			return result - 1;

		result = ModPow(n, MOD - 2);
		if (n < _inverse.Length)
			_inverse[n] = (int)(result + 1);
		return result;
	}

	public static long Mult(long left, long right)
	{
		return (left * right) % MOD;
	}

	public static long Div(long left, long divisor)
	{
		return left % divisor == 0
			? left / divisor
			: Mult(left, Inverse(divisor));
	}

	public static long Subtract(long left, long right)
	{
		return (left + (MOD - right)) % MOD;
	}

	public static long Fix(long m)
	{
		var result = m % MOD;
		if (result < 0) result += MOD;
		return result;
	}

	public static long ModPow(long n, long p, long mod = MOD)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = (result * b) % mod;
			p >>= 1;
			b = (b * b) % mod;
		}
		return result;
	}

	public static long Pow(long n, long p)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result *= b;
			p >>= 1;
			b *= b;
		}
		return result;
	}

	#endregion

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
	        if (number < 0) throw new FormatException();
        }
        return neg ? -number : number;
    }

    public static long Nl()
    {
        var c = SkipSpaces();
        bool neg = c=='-';
        if (neg) { c = Read(); }

        long number = c - '0';
        while (true)
        {
            var d = Read() - '0';
            if ((uint)d > 9) break;
            number = number * 10 + d;
	        if (number < 0) throw new FormatException();
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