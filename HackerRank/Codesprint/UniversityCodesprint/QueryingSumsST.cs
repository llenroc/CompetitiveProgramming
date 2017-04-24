namespace HackerRank.UniversityCodesprint2.QueryingSumsST
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

            var st = new SuffixTree(_s);
            _suffixes = st.BuildSuffixArray();
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

    public class SuffixTree
    {
        public readonly char[] Text;
        public readonly Node Root;

        Node _lastNewNode;
        Node _activeNode;

        int _activeEdge;
        int _activeLength;

        int _remainingSuffixCount;
        int _size;

        public readonly int _charStart;
        public readonly int _charRange;
        readonly EndHolder _leafEnd = new EndHolder(-1);
        EndHolder _splitEnd;

        public SuffixTree(char[] text)
        {
            Text = text;

            _charStart = text.Length == 0 ? 0 : int.MaxValue;
            var charEnd = 0;
            foreach (var c in text)
            {
                _charStart = Min(_charStart, c);
                charEnd = Max(charEnd, c);
            }
            _charRange = charEnd - _charStart + 1;

            _activeEdge = -1;
            _size = text.Length;
            _activeNode = Root = new Node(this, -1, new EndHolder(-1));
            for (int i = 0; i < _size; i++)
                ExtendSuffixTree(i);
            int labelHeight = 0;
            SetSuffixIndexByDfs(Root, labelHeight);
        }

        bool WalkDown(Node currNode)
        {
            if (_activeLength < currNode.EdgeLength) return false;
            _activeEdge += currNode.EdgeLength;
            _activeLength -= currNode.EdgeLength;
            _activeNode = currNode;
            return true;
        }

        void ExtendSuffixTree(int pos)
        {
            _leafEnd.Value = pos;

            _remainingSuffixCount++;
            _lastNewNode = null;

            while (_remainingSuffixCount > 0)
            {
                if (_activeLength == 0)
                    _activeEdge = pos;

                if (_activeNode[Text[_activeEdge]] == null)
                {
                    //Extension Rule 2 (A new leaf edge gets created)
                    _activeNode[Text[_activeEdge]] = new Node(this, pos, _leafEnd);

                    if (_lastNewNode != null)
                    {
                        _lastNewNode.Link = _activeNode;
                        _lastNewNode = null;
                    }
                }
                else
                {
                    Node next = _activeNode[Text[_activeEdge]];
                    if (WalkDown(next))
                        continue;

                    if (Text[next.Start + _activeLength] == Text[pos])
                    {
                        if (_lastNewNode != null && _activeNode != Root)
                        {
                            _lastNewNode.Link = _activeNode;
                            _lastNewNode = null;
                        }

                        _activeLength++;
                        break;
                    }

                    _splitEnd = new EndHolder(next.Start + _activeLength - 1);

                    var split = new Node(this, next.Start, _splitEnd);
                    _activeNode[Text[_activeEdge]] = split;

                    split[Text[pos]] = new Node(this, pos, _leafEnd);
                    next.Start += _activeLength;
                    split[Text[next.Start]] = next;

                    if (_lastNewNode != null)
                        _lastNewNode.Link = split;

                    _lastNewNode = split;
                }

                _remainingSuffixCount--;
                if (_activeNode == Root && _activeLength > 0) //APCFER2C1
                {
                    _activeLength--;
                    _activeEdge = pos - _remainingSuffixCount + 1;
                }
                else if (_activeNode != Root) //APCFER2C2
                {
                    _activeNode = _activeNode.Link;
                }
            }
        }

        void SetSuffixIndexByDfs(Node n, int labelHeight)
        {
            if (n == null)
                return;

            bool leaf = true;
            foreach (var c in n.Children)
            {
                if (c != null)
                {
                    leaf = false;
                    SetSuffixIndexByDfs(c, labelHeight + c.EdgeLength);
                }
            }

            if (leaf)
            {
                //for (int i = n.Start; i <= n.End; i++)
                //{
                //    if (Text[i] == '#') //Trim unwanted characters
                //        n.End = i;
                //}

                n.SuffixIndex = _size - labelHeight;
            }
        }

        public void OutputSuffixTree(Node n, StringBuilder builder)
        {
            if (n == null)
                return;

            if (n.Start != -1)
                builder.Append($"{n.Start} {n.End} ");

            bool leaf = true;
            foreach (var c in n.Children)
            {
                if (c != null)
                {
                    leaf = false;
                    OutputSuffixTree(c, builder);
                }
            }

            if (leaf)
                builder.AppendLine($" {n.SuffixIndex}");
        }

        /// <summary>
        /// Returns a view of the trie
        /// </summary>
        /// <returns></returns>
	    public override string ToString()
        {
            var builder = new StringBuilder();
            OutputSuffixTree(Root, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Determines whether substring is contain in the trie string
        /// </summary>
        /// <param name="substring"></param>
        /// <returns></returns>
        public bool ContainsSubstring(string substring)
        {
            int index;
            return FindNode(substring, out index) != null;
        }

        /// <summary>
        /// Finds the node containing the substring and returns ending position+1
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="index">1+the position of ending char in the node</param>
        /// <returns>the found node</returns>
        public Node FindNode(string substring, out int index)
        {
            index = 0;

            Node n = Root;
            int i = 0;
            while (n != null)
            {
                if (n.Start != -1)
                {
                    int j = i;
                    int k = n.Start;
                    for (; k <= n.End && j < substring.Length; k++, j++)
                        if (Text[k] != substring[j])
                            return null;

                    if (j == substring.Length)
                    {
                        index = k;
                        return n;
                    }
                }
                i = i + n.EdgeLength;
                n = n[substring[i]];
            }

            return null;
        }

        private void Dfs(Node n, Func<Node, bool> predicate)
        {
            var stack = new Stack<Node>();
            stack.Push(n);

            while (stack.Count > 0)
            {
                var pop = stack.Pop();
                if (predicate(pop))
                    return;
                PushChildren(stack, pop);
            }
        }

        void PushChildren(Stack<Node> stack, Node n)
        {
            var children = n.Children;
            for (int i = children.Length - 1; i >= 0; i--)
            {
                var c = children[i];
                if (c != null)
                    stack.Push(c);
            }
        }

        /// <summary>
        /// Returns the start indices of all occurrences of the substring
        /// </summary>
        /// <param name="substring"></param>
        /// <returns></returns>
        public IEnumerable<int> IndicesOf(string substring)
        {
            int index;
            Node n = FindNode(substring, out index);
            if (n == null) yield break;

            var stack = new Stack<Node>();
            stack.Push(n);
            while (stack.Count > 0)
            {
                var pop = stack.Pop();
                if (pop.SuffixIndex >= 0)
                {
                    yield return pop.SuffixIndex;
                    continue;
                }
                PushChildren(stack, pop);
            }
        }

        void DfsRepeatedSubstring(Node n, int length, ref int start, ref int maxLength)
        {
            if (n == null)
                return;

            if (n.SuffixIndex == -1) //If it is internal node
            {
                foreach (var c in n.Children)
                    if (c != null)
                        DfsRepeatedSubstring(c, length + c.EdgeLength, ref start, ref maxLength);
            }
            else if (n.SuffixIndex >= 0 && maxLength < length - n.EdgeLength)
            {
                maxLength = length - n.EdgeLength;
                start = n.SuffixIndex;
            }
        }

        /// <summary>
        /// Finds the longest repeated substring in text
        /// </summary>
        /// <param name="start">position of some longest repeated string </param>
        /// <param name="maxLength">length of longest repeated string or 0 if none</param>
        public void LongestRepeatedSubstring(out int start, out int maxLength)
        {
            start = 0;
            maxLength = 0;
            DfsRepeatedSubstring(Root, 0, ref start, ref maxLength);
        }


        #region Build Suffix Array

        void DfsSuffixArray(Node n, int[] suffixArray, ref int idx)
        {
            if (n == null)
                return;

            if (n.SuffixIndex == -1) //If it is internal node
            {
                foreach (var c in n.Children)
                    if (c != null)
                        DfsSuffixArray(c, suffixArray, ref idx);
            }
            //If it is Leaf node other than "$" label
            else if (n.SuffixIndex > -1 && n.SuffixIndex < _size)
            {
                suffixArray[idx++] = n.SuffixIndex;
            }
        }

        public int[] BuildSuffixArray()
        {
            _size--;
            int[] suffixArray = new int[_size];
            for (int i = 0; i < _size; i++)
                suffixArray[i] = -1;
            int idx = 0;
            DfsSuffixArray(Root, suffixArray, ref idx);
            _size++;
            return suffixArray;
        }

        #endregion

        #region Helpers
        public class Node
        {
            public readonly SuffixTree Tree;
            public Node[] _children;
            public Node Link;
            public int Start;
            EndHolder _endHolder;

            public Node[] Children => _children ?? Array.Empty<Node>();

            public int End
            {
                get { return _endHolder.Value; }
                set { _endHolder = new EndHolder(value); }
            }

            public int SuffixIndex = -1;

            public Node this[char index]
            {
                get
                {
                    int i = index - Tree._charStart;
                    if (_children != null && (uint)i < (uint)(Tree._charRange))
                        return _children[i];
                    return null;
                }
                set
                {
                    if (_children == null)
                        _children = new Node[Tree._charRange];
                    _children[index - Tree._charStart] = value;
                }
            }

            public Node(SuffixTree tree, int start, EndHolder endHolder)
            {
                Tree = tree;
                Link = tree.Root;
                Start = start;
                _endHolder = endHolder;
            }

            public int EdgeLength => this == Tree.Root ? 0 : _endHolder.Value - Start + 1;
        }

        public class EndHolder
        {
            public int Value;
            public EndHolder(int value)
            {
                Value = value;
            }
        }
        #endregion
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