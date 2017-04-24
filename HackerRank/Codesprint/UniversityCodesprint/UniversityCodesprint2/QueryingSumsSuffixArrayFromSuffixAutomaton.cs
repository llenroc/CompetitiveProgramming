namespace HackerRank.UniversityCodesprint2.QueryingSumsSuffixArrayFromSuffixAutomaton
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using static System.Math;
    using System.Text;
    using System.Diagnostics;
    using static FastIO;

    public class Solution
    {
        #region Variables

        static char[] _s;
        static int _m;
        static int _q;
        static int _k;
        static Pair[] _pairs;
        static RangeMinimumQuery _rmq;
        static int[] _lcps;
        static int[] _suffixes;
        static int[] _indices;
        #endregion

        public static void Main()
        {
            InitIO();
            int n = Ni();
            _m = Ni();
            _q = Ni();
            _k = Ni();

            int concatLength = (n + 1) + (_k + 1) * _q;
            _s = new char[concatLength];
            Nc(_s, 0, n);
            int idx = n;
            _s[idx++] = '{';

            _pairs = new Pair[_m];

            for (int i = 0; i < _m; i++)
                _pairs[i] = new Pair { Index = i, Start = Ni(), End = Ni() };

            Array.Sort(_pairs);

            var queries = new List<Query>();
            for (int a0 = 0; a0 < _q; a0++)
            {
                string w = Ns();
                int a = Ni();
                int b = Ni();

                var query = new Query
                {
                    Word = w,
                    A = a,
                    B = b,
                    QueryIndex = a0,
                    WordIndex = idx
                };
                queries.Add(query);

                query.Word.CopyTo(0, _s, idx, _k);
                idx += _k;
                _s[idx++] = '|';
            }
            _s[idx - 1] = '}';

            var st = new SuffixAutomaton(_s);
            _suffixes = st.GetSuffixArray();
            _indices = LongestCommonPrefixes.BuildIndices(_suffixes);
            _lcps = LongestCommonPrefixes.FromSuffixArray(_s, _suffixes, _indices);

            _rmq = new RangeMinimumQuery(_lcps);

            int[] presum = new int[_s.Length - 1];
            int sum = 0;
            for (int i = 0; i < presum.Length; i++)
            {
                presum[i] = sum;
                if (_suffixes[i] < n) sum++;
            }

            var tmp = new int[_m];
            foreach (var query in queries)
            {
                long answer = 0;

                for (int i = 0; i < _m; i++)
                {
                    var p = _pairs[i];
                    if (i > 0)
                    {
                        var prev = _pairs[i - 1];
                        if (prev.End == p.End && prev.Start == p.Start)
                        {
                            tmp[p.Index] = tmp[prev.Index];
                            continue;
                        }
                    }

                    int left = p.Start;
                    int right = p.End;
                    int len = right - left + 1;
                    int leftSuffix = _indices[query.WordIndex + left];
                    int rightSuffix = leftSuffix + 1;

                    /*
					var leftSuffixOrig = leftSuffix;
					var rightSuffixOrig = rightSuffix;


					var leftSuffix2 = leftSuffix;
					var rightSuffix2 = rightSuffix;
					while (lcps[rightSuffix2] >= len)
						rightSuffix2++;
					while (lcps[leftSuffix2] >= len && leftSuffix2 >= 0)
						leftSuffix2--;
						*/

                    Find(_lcps, len, ref leftSuffix, ref rightSuffix);

                    /*
					Debug.Assert(leftSuffix == leftSuffix2);
					Debug.Assert(rightSuffix == rightSuffix2);
					*/

                    tmp[p.Index] = presum[rightSuffix] - presum[leftSuffix];
                }

                sum = 0;
                for (int i = 0; i < _m; i++)
                    sum = tmp[i] = sum + tmp[i];

                answer += tmp[query.B] - (query.A > 0 ? tmp[query.A - 1] : 0);
                Console.WriteLine(answer);
            }
        }

        public class Pair : IComparable<Pair>
        {
            public int Start;
            public int End;
            public int Index;

            public int CompareTo(Pair b)
            {
                int cmp = Start.CompareTo(b.Start);
                if (cmp != 0)
                    return cmp;
                cmp = End.CompareTo(b.End);
                return cmp;
            }
        }


        static void Find(int[] lcps, int len, ref int left, ref int right)
        {
            int point = right;
            if (lcps[right] >= len)
            {
                int dist = 1;
                while (right + dist < lcps.Length && _rmq.GetMin(point, right + dist) >= len)
                {
                    right += dist;
                    dist <<= 1;
                }

                for (; dist > 0; dist >>= 1)
                {
                    while (right + dist < lcps.Length && _rmq.GetMin(point, right + dist) >= len)
                        right += dist;
                }

                if (lcps[right + 1] < len)
                    right++;
            }


            point = left;
            if (lcps[left] >= len)
            {
                int dist = 1;
                while (left - dist >= 0 && _rmq.GetMin(left - dist + 1, point) >= len)
                {
                    left -= dist;
                    dist <<= 1;
                }

                for (; dist > 0; dist >>= 1)
                {
                    while (left - dist >= 0 && _rmq.GetMin(left - dist + 1, point) >= len)
                        left -= dist;
                }
            }

        }

        class Query
        {
            public int QueryIndex;
            public int WordIndex;
            public string Word;
            public int A;
            public int B;

        }


#if DEBUG
        public static bool Verbose = true;
#else
		public static bool Verbose = false;
#endif
    }

    public class RangeMinimumQuery
    {
        readonly int[,] _table;
        readonly int _n;
        readonly int[] _array;

        public RangeMinimumQuery(int[] array)
        {
            _array = array;
            _n = array.Length;

            int n = array.Length;
            int lgn = Log2(n);
            _table = new int[lgn, n];

            _table[0, n - 1] = n - 1;
            for (int j = n - 2; j >= 0; j--)
                _table[0, j] = array[j] <= array[j + 1] ? j : j + 1;

            for (int i = 1; i < lgn; i++)
            {
                int curlen = 1 << i;
                for (int j = 0; j < n; j++)
                {
                    int right = j + curlen;
                    var pos1 = _table[i - 1, j];
                    int pos2;
                    _table[i, j] =
                        (right >= n || array[pos1] <= array[pos2 = _table[i - 1, right]])
                            ? pos1
                            : pos2;
                }
            }
        }

        public int GetArgMin(int left, int right)
        {
            if (left == right) return left;
            int curlog = Log2(right - left + 1);
            int pos1 = _table[curlog - 1, left];
            int pos2 = _table[curlog - 1, right - (1 << curlog) + 1];
            return _array[pos1] <= _array[pos2] ? pos1 : pos2;
        }

        public int GetMin(int left, int right)
        {
            return _array[GetArgMin(left, right)];
        }


        static int Log2(int value)
        {
            var log = 0;
            if ((uint)value >= (1U << 12))
            {
                log = 12;
                value = (int)((uint)value >> 12);
                if (value >= (1 << 12))
                {
                    log += 12;
                    value >>= 12;
                }
            }
            if (value >= (1 << 6))
            {
                log += 6;
                value >>= 6;
            }
            if (value >= (1 << 3))
            {
                log += 3;
                value >>= 3;
            }
            return log + (value >> 1 & ~value >> 2);
        }
    }

	public class SuffixAutomaton
	{
		public Node Start;
		public Node End;
		public int NodeCount;

		Node[] _nodes;
		SummarizedState[] _summary;

		private SuffixAutomaton()
		{
			Start = new Node();
			End = Start;
			NodeCount = 1;
		}

		public SuffixAutomaton(IEnumerable<char> s) : this()
		{
			foreach (var c in s)
				Extend(c);

			for (var p = End; p != Start; p = p.Link)
				p.IsTerminal = true;
		}

		public void Extend(char c)
		{
			var node = new Node
			{
				Key = c,
				Len = End.Len + 1,
				Link = Start,
			};
			NodeCount++;

			Node p;
			for (p = End; p != null && p[c] == null; p = p.Link)
				p[c] = node;
			End = node;

			if (p == null) return;

			var q = p[c];
			if (p.Len + 1 == q.Len)
				node.Link = q;
			else
			{
				var clone = q.Clone();
				clone.Len = p.Len + 1;
				NodeCount++;

				for (; p != null && p[c] == q; p = p.Link)
					p[c] = clone;

				q.Link = node.Link = clone;
			}
		}

		public bool ContainsSubstring(string s)
		{
			return FindNode(s) != null;
		}

		public Node[] GetNodes()
		{
			if (_nodes != null && NodeCount == _nodes.Length)
				return _nodes;

			var nodes = _nodes = new Node[NodeCount];
			int stack = 0;
			int idx = NodeCount;

			nodes[stack++] = Start;
			while (stack > 0)
			{
				var current = nodes[--stack];

				if (current.Index > 0)
					current.Index = 0;

				current.Index--;
				var index = current.NextCount + current.Index;
				if (index >= 0)
				{
					stack++;

					var child = current.Next[index];
					if (child.Index >= -child.NextCount)
						nodes[stack++] = current.Next[index];
				}
				else if (index == -1)
				{
					nodes[--idx] = current;
				}
				Debug.Assert(idx >= stack);
			}

			if (idx != 0)
			{
				Debug.Assert(idx == 0, "NodeCount smaller than number of nodes");
				NodeCount -= idx;
				_nodes = new Node[NodeCount];
				Array.Copy(nodes, idx, _nodes, 0, NodeCount);
			}

			UpdateNodeIndices();
			return _nodes;
		}


		public IEnumerable<Node> NodesBottomUp()
		{
			var nodes = GetNodes();
			for (int i = NodeCount - 1; i >= 0; i--)
				yield return nodes[i];
		}

		void UpdateNodeIndices()
		{
			var nodes = _nodes;
			for (int i = 0; i < NodeCount; i++)
				nodes[i].Index = i;
		}

		public Node[] SortTopologically()
		{
			int[] indeg = new int[NodeCount];
			var nodes = GetNodes();
			for (int i = 0; i < NodeCount; i++)
			{
				Node cur = nodes[i];
				for (int j = 0; j < cur.NextCount; j++)
					indeg[cur.Next[j].Index]++;
			}

			var sorted = new Node[NodeCount];
			sorted[0] = Start;
			int p = 1;
			for (int i = 0; i < NodeCount; i++)
			{
				Node cur = sorted[i];
				for (int j = 0; j < cur.NextCount; j++)
				{
					if (--indeg[cur.Next[j].Index] == 0)
						sorted[p++] = cur.Next[j];
				}
			}

			_nodes = sorted;
			UpdateNodeIndices();
			return sorted;
		}

		public Node FindNode(string pattern, int index, int count)
		{
			var node = Start;
			for (int i = 0; i < count; i++)
			{
				node = node[pattern[index + i]];
				if (node == null) return null;
			}
			return node;
		}

		public Node FindNode(string pattern)
		{
			return FindNode(pattern, 0, pattern.Length);
		}

		public Node FindNode(IEnumerable<char> pattern)
		{
			var node = Start;
			foreach (var ch in pattern)
			{
				node = node[ch];
				if (node == null) return null;
			}
			return node;
		}

		public void LongestCommonSubstring(string t, out int start, out int length)
		{
			start = 0;
			length = 0;
			if (t.Length == 0) return;
			Node v = Start;
			int len = 0;
			for (int i = 0; i < t.Length; i++)
			{
				while (v != Start && v[t[i]] == null)
				{
					v = v.Link;
					len = v.Len;
				}

				if (v[t[i]] != null)
				{
					v = v[t[i]];
					len++;
				}

				if (len > length)
				{
					length = len;
					start = i;
				}
			}

			start += 1 - length;
		}

		public string LongestCommonSubstring(string t)
		{
			int start;
			int length;
			LongestCommonSubstring(t, out start, out length);
			return t.Substring(start, length);
		}


		public SummarizedState[] SummarizedAutomaton()
		{
			if (_summary != null)
				return _summary;

			var summary = new SummarizedState[NodeCount];
			foreach (var n in NodesBottomUp())
			{

				if (n.NextCount == 1 && !n.IsTerminal)
				{
					var c = summary[n.Next[0].Index];
					summary[n.Index] = new SummarizedState { Node = c.Node, Length = c.Length + 1 };
				}
				else
				{
					summary[n.Index] = new SummarizedState { Node = n, Length = 1 };
				}
			}

			_summary = summary;
			return summary;
		}

		public struct SummarizedState
		{
			public Node Node;
			public int Length;
			public override string ToString() => $"Node={Node?.Index} Length={Length}";
		}


		public int[] GetSuffixArray()
		{
			var summary = SummarizedAutomaton();
			int[] suffixArray = new int[End.Len];
			int idx = 0;
			DfsSuffixArray(Start, suffixArray, 0, ref idx);
			return suffixArray;
		}

		void DfsSuffixArray(Node node,
			int[] suffixArray,
			int length,
			ref int idx)
		{
			SummarizedState c;

			while (node.IsTerminal)
			{
				suffixArray[idx++] = End.Len - length;
				if (node.NextCount != 1) break;
				c= _summary[node.Next[0].Index];
				node = c.Node;
				length += c.Length;
			}

			for (var index = 0; index < node.NextCount; index++)
			{
				c = _summary[node.Next[index].Index];
				DfsSuffixArray(c.Node, suffixArray, length + c.Length, ref idx);
			}
		}

		/// <summary>
		/// Finds the longest repeated substring in text
		/// </summary>
		/// <param name="start">position of some longest repeated string </param>
		/// <param name="maxLength">length of longest repeated string or 0 if none</param>
		public void LongestRepeatedSubstring(out int start, out int maxLength)
		{
			var summary = SummarizedAutomaton();
			start = 0;
			maxLength = 0;
			DfsRepeatedSubstring(Start, 0, summary, ref start, ref maxLength);
		}

		void DfsRepeatedSubstring(Node n, int length, SummarizedState[] summary, ref int start, ref int maxLength)
		{
			int count = n.NextCount + (n.IsTerminal ? 1 : 0);

			if (count >= 2 && length > maxLength)
			{
				maxLength = length;
				start = End.Len - length;
			}

			foreach (var c in n.Children)
				DfsRepeatedSubstring(
					summary[c.Index].Node,
					length + summary[c.Index].Length,
					summary,
					ref start, ref maxLength);
		}


		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var n in GetNodes())
			{
				if (n == null) continue;
				sb.Append(string.Format("{id:%d, len:%d, link:%d, cloned:%b, ", n.Index, n.Len, n.Link?.Index ?? -1, n.IsCloned));
				sb.Append("Next:{");
				foreach (var c in n.Children)
					sb.Append(c.Key + ":" + c.Index + ",");
				sb.Append("}}\n");
			}
			return sb.ToString();
		}

		public class Node
		{
			public char Key;
			public bool IsTerminal;
			public byte NextCount;
			public int Len;
			public int Index;
			int KeyMask;
			public Node Link;
			public Node Original;
			public Node[] Next;

			public Node()
			{
				Next = Array.Empty<Node>();
			}

			public Node this[char ch]
			{
				get
				{
					if ((KeyMask << ~ch) < 0)
					{
						int left = 0;
						int right = NextCount - 1;
						while (left <= right)
						{
							int mid = (left + right) >> 1;
							var val = Next[mid];
							int cmp = val.Key - ch;
							if (cmp < 0)
								left = mid + 1;
							else if (cmp > 0)
								right = mid - 1;
							else
								return val;
						}
					}
					return null;
				}
				set
				{
					int left = 0;
					int right = NextCount - 1;
					while (left <= right)
					{
						int mid = (left + right) >> 1;
						var val = Next[mid];
						int cmp = val.Key - ch;
						if (cmp < 0)
							left = mid + 1;
						else if (cmp > 0)
							right = mid - 1;
						else
						{
							Next[mid] = value;
							return;
						}
					}

					if (NextCount >= Next.Length)
						Array.Resize(ref Next, Max(2, NextCount * 2));
					if (NextCount > left)
						Array.Copy(Next, left, Next, left + 1, NextCount - left);
					NextCount++;
					Next[left] = value;
					KeyMask |= 1 << ch;
				}
			}

			public bool IsCloned => Original != null;

			public IEnumerable<Node> Children
			{
				get
				{
					for (int i = 0; i < NextCount; i++)
						yield return Next[i];
				}
			}

			public Node Clone()
			{
				var node = (Node)MemberwiseClone();
				node.Original = this;
				node.Next = (Node[])node.Next.Clone();
				return node;
			}

		}
	}

	public class LongestCommonPrefixes
    {

        public static int[] BuildIndices(int[] suffixArray)
        {
            int[] indices = new int[suffixArray.Length];
            for (int i = 0; i < suffixArray.Length; i++)
                indices[suffixArray[i]] = i;
            return indices;
        }

        public static int[] FromSuffixArray(CharSequence txt,
            int[] suffixArray,
            int[] indices = null)
        {
            int n = suffixArray.Length;
            if (indices == null)
                indices = BuildIndices(suffixArray);

            int[] lcp = new int[n];

            int k = 0;

            for (int i = 0; i < n; i++)
            {
                if (indices[i] == n - 1)
                {
                    k = 0;
                    continue;
                }

                int j = suffixArray[indices[i] + 1];

                while (i + k < n && j + k < n && txt[i + k] == txt[j + k])
                    k++;

                lcp[indices[i] + 1] = k;

                if (k > 0)
                    k--;
            }

            return lcp;
        }
    }

    public abstract class CharSequence : IEnumerable<char>
    {

        #region Properties
        public abstract char this[int index]
        {
            get;
        }

        public abstract object Data
        {
            get;
        }

        public abstract int Length
        {
            get;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return Data.ToString();
        }
        #endregion


        #region Collection
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<char> GetEnumerator()
        {
            int length = Length;
            for (int i = 0; i < length; i++)
                yield return this[i];
        }
        #endregion


        #region Converters
        public static implicit operator CharSequence(char[] ch)
        {
            return new ArrayBased(ch);
        }

        public static implicit operator CharSequence(string str)
        {
            return new StringBased(str);
        }

        public static implicit operator CharSequence(StringBuilder str)
        {
            return new StringBuilderBased(str);
        }
        #endregion


        #region Helpers
        class StringBased : CharSequence
        {
            readonly string _data;

            public StringBased(string s)
            {
                _data = s;
            }

            public override char this[int index] => _data[index];
            public override int Length => _data.Length;
            public override object Data => _data;
        }


        class StringBuilderBased : CharSequence
        {
            readonly StringBuilder _data;

            public StringBuilderBased(StringBuilder s)
            {
                _data = s;
            }

            public override char this[int index] => _data[index];
            public override int Length => _data.Length;
            public override object Data => _data;
        }


        class ArrayBased : CharSequence
        {
            readonly char[] _data;

            public ArrayBased(char[] s)
            {
                _data = s;
            }

            public override char this[int index] => _data[index];
            public override int Length => _data.Length;
            public override object Data => _data;
            public override string ToString()
            {
                return new string(_data);
            }
        }
        #endregion

    }

    public static class FastIO
    {
        static System.IO.Stream stream;
        static int idx, bytesRead;
        static byte[] buffer;
        static System.Text.StringBuilder builder;
        static System.IO.TextWriter Out;


        public static void InitIO(
            int stringCapacity = 16,
            int bufferSize = 1 << 20,
            System.IO.Stream input = null)
        {
            builder = new System.Text.StringBuilder(stringCapacity);
            stream = input ?? Console.OpenStandardInput();
            idx = bytesRead = 0;
            buffer = new byte[bufferSize];
            Out = new System.IO.StreamWriter(Console.OpenStandardOutput());
        }


        static void ReadMore()
        {
            idx = 0;
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead <= 0) buffer[0] = 32;
        }

        public static int Read()
        {
            if (idx >= bytesRead) ReadMore();
            return buffer[idx++];
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
            if (neg)
            {
                c = Read();
            }

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
            if (neg)
            {
                c = Read();
            }

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

        public static void Nc(char[] list, int start, int n)
        {
            for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[start++] = (char)c;
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

    }
}