//namespace HackerRank.WeekOfCode31.Problem
// Powered by caide (code generator, tester, and library code inliner)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static FastIO;

public class Solution
{
	public void solve(Stream input, Stream output)
	{
		InitInput(input);
		InitOutput(output);
		solve();
		Flush();
	}

	char[] ca;

	public void solve()
	{
		int n = Ni();
		ca = Nc(n);

		var pt = new PalindromeTree(n);
		var trie = new SimpleTrie();

		long prefixes = 0;
		for (var ich = 0; ich < ca.Length; ich++)
		{
			var ch = ca[ich];
			var node = pt.Extend(ch);

			if (node != null)
			{
				SimpleTrie t = trie;
				var suffix = node.SuffixLink;
				int suffixLength = 0;
				if (suffix != null && suffix.Length>=1)
				{
					t = suffix.Trie;
					suffixLength = suffix.Length;
				}

				int prefixLength = 0;
				for (int i = suffixLength; i < node.Length; i++)
				{
					var c = ca[ich - i];
					var next = t.MoveNext(c);
					if (next == null)
					{
						prefixLength++;
						next = t.MoveNext(c, true);
					}
					t = next;
				}
				prefixes += prefixLength;
				node.Trie = t;
			}

			long ans = prefixes;
			//long ans = pt.Nodes.Count - 2 + prefixes;
			WriteLine(ans);
		}
	}
}


public class PalindromeTree
{
	public StringBuilder Text;
	public List<Node> Nodes;
	public Node Root;
	public Node Empty;
	Node suffix;

	public PalindromeTree(int length)
	{
		Text = new StringBuilder(length);
		Root = new Node { Length = -1 };
		Empty = new Node { Length = 0, SuffixLink = Root };
		Root.SuffixLink = Root;

		suffix = Empty;

		Nodes = new List<Node>(length)
		{
			Root,
			Empty,
		};
	}

	public Node Extend(char ch)
	{
		int pos = Text.Length;
		Text.Append(ch);

		Node current;
		int pos2;
		for (current = suffix; ; current = current.SuffixLink)
		{
			pos2 = pos - 1 - current.Length;
			if (pos2 >= 0 && Text[pos2] == ch)
				break;
		}

		int let = ch - 'a';
		if (current.Next[let] != null)
		{
			// We found an existing palindrome
			suffix = current.Next[let];
			return null;
		}

		var node = suffix = new Node { Length = current.Length + 2 };
		current.Next[let] = node;
		Nodes.Add(node);

		if (node.Length == 1)
		{
			// Single character palindrome
			node.SuffixLink = Empty;
			node.Count = 1;
			return node;
		}

		do
		{
			current = current.SuffixLink;
			pos2 = pos - 1 - current.Length;
		}
		while (pos2 < 0 || Text[pos2] != ch);

		node.SuffixLink = current.Next[let];
		node.Count = 1 + node.SuffixLink.Count;
		return node;
	}


	public class Node
	{
		public Node[] Next = new Node[26];
		public Node SuffixLink;
		public int Length;
		public int Count;
		public SimpleTrie Trie;
	}
}

public class SimpleTrie
{
	public char Ch;
	public bool EndOfWord;
	public int Size;
	public int WordCount;

	public SimpleTrie NextSibling;
	public SimpleTrie FirstChild;


	public SimpleTrie MoveNext(char ch, bool create = false)
	{
		SimpleTrie prev = null;
		SimpleTrie newChild = null;
		SimpleTrie child = FirstChild;
		for (; child != null; prev = child, child = child.NextSibling)
		{
			if (child.Ch > ch) break;
			if (child.Ch == ch) return child;
		}

		if (create)
		{
			newChild = new SimpleTrie
			{
				Ch = ch,
				NextSibling = child
			};
			if (prev == null)
				FirstChild = newChild;
			else
				prev.NextSibling = newChild;
		}
		return newChild;
	}


	public IEnumerable<SimpleTrie> GetChildren()
	{
		for (var child = FirstChild; child != null; child = child.NextSibling)
			yield return child;
	}

	public IEnumerable<SimpleTrie> GetChildrenWords()
	{

		for (var child = FirstChild; child != null; child = child.NextSibling)
			yield return child;
	}


}



public static class FastIO
{
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

public static class Parameters
{
#if DEBUG
	public const bool Verbose = true;
#else
	public const bool Verbose = false;
#endif
}

class CaideConstants {
    public const string InputFile = null;
    public const string OutputFile = null;
}
public class Program {
    public static void Main(string[] args)
    {
        Solution solution = new Solution();
        solution.solve(Console.OpenStandardInput(), Console.OpenStandardOutput());

#if DEBUG
		Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
	}
}

