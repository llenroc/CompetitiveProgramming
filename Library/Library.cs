using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Library
{
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
		var result = left - right;
		if (result < 0) result += MOD;
		return result;
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

	#region Combinatorics
	static List<long> _fact;
	static List<long> _ifact;

	public static long Fact(int n)
	{
		if (_fact == null) _fact = new List<long>(100) { 1 };
		for (int i = _fact.Count; i <= n; i++)
			_fact.Add(Mult(_fact[i - 1], i));
		return _fact[n];
	}

	public static long InverseFact(int n)
	{
		if (_ifact == null) _ifact = new List<long>(100) { 1 };
		for (int i = _ifact.Count; i <= n; i++)
			_ifact.Add(Div(_ifact[i - 1], i));
		return _ifact[n];
	}

	public static long Comb(int n, int k)
	{
		if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
		if (k + k > n) return Comb(n, n - k);
		return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
	}

    #endregion

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

    #region Comparer

    public class Comparer<T> : IComparer<T>
	{
		readonly Comparison<T> _comparison;

		public Comparer(Comparison<T> comparison)
		{
			_comparison = comparison;
		}

		public int Compare(T a, T b) => _comparison(a, b);
	}

    #endregion

    #region Reporting Answer

    static System.Threading.Timer _timer;
	static Func<bool> _timerAction;
    public static System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
    public static double Elapsed => process.TotalProcessorTime.TotalMilliseconds;

    public static void LaunchTimer(Func<bool> action, long ms = 2900)
	{
		_timerAction = action;
	    ms -= (long) Elapsed + 1;

		_timer = new System.Threading.Timer(
			delegate
			{
#if !DEBUG
                if (_timerAction())
					Environment.Exit(0);
#endif
			}, null, ms, 0);
	}

	public static void Run(string name, Action action)
	{
#if DEBUG
		Console.Write(name + ": ");
		var start = Elapsed;
		action();
		var elapsed = Elapsed - start;
		Console.WriteLine($"Elapsed Time: {elapsed}\n");
#else
		action();
#endif
	}

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Run2(string name, Action action) => GC.KeepAlive(name);

    #endregion

    #region  Parameters
    public const int StringCapacity = 16;

#if DEBUG
    public static bool Verbose = true;
#else
	public static bool Verbose = false;
#endif
    #endregion
}

