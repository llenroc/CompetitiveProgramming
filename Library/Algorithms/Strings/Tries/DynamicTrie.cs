﻿using System;
using System.Collections.Generic;
using System.IO;
using Softperson.Collections;

namespace Softperson.Algorithms.Strings
{
	public class DynamicTrie : IEquatable<DynamicTrie>
	{
		private static readonly TrieData NullTrie = new TrieData();
		public int Hashcode;
		public bool IsWord;
		public DynamicTrie Left;
		public char Letter;
		public DynamicTrie Middle;
		public DynamicTrie Right;

		public bool Equals(DynamicTrie t)
		{
			if (t == this)
				return true;
			if (t == null)
				return false;

			return t.Letter == Letter
				   && t.GetHashCode() == GetHashCode()
				   && t.IsWord == IsWord
				   && Utility.Equals(ref t.Left, ref Left)
				   && Utility.Equals(ref t.Middle, ref Middle)
				   && Utility.Equals(ref t.Right, ref Right);
		}

		public DynamicTrie Clone()
		{
			return (DynamicTrie) MemberwiseClone();
		}

		public static void Insert(ref DynamicTrie trie, string s)
		{
			if (s.Length != 0)
			{
				Insert(ref trie, s, 0);
			}
		}

		private static bool Insert(ref DynamicTrie trie, string s, int pos)
		{
			if (pos >= s.Length)
				return false;

			var ch = char.ToUpper(s[pos]);
			var changed = false;

			if (trie == null)
			{
				trie = new DynamicTrie {Letter = ch};
				changed = true;
			}

			if (trie.Letter == ch)
			{
				if (pos + 1 < s.Length)
				{
					changed |= Insert(ref trie.Middle, s, pos + 1);
				}
				else if (trie.IsWord)
				{
					return false;
				}
				else
				{
					changed = true;
					trie.IsWord = true;
				}
			}
			else if (trie.Letter < ch)
			{
				changed = Insert(ref trie.Left, s, pos);
			}
			else
			{
				changed = Insert(ref trie.Right, s, pos);
			}

			if (changed)
				trie.Hashcode = 0;
			return changed;
		}

		public override int GetHashCode()
		{
			return GetHashCode(this);
		}

		public override bool Equals(object obj)
		{
			var t = obj as DynamicTrie;
			return Equals(t);
		}

		public static int GetHashCode(DynamicTrie trie)
		{
			if (trie == null)
				return 0;

			if (trie.Hashcode == 0)
			{
				var hashcode = GetHashCode(trie.Middle);
				hashcode = Utility.CreateHashCode(hashcode, GetHashCode(trie.Left));
				hashcode = Utility.CreateHashCode(hashcode, GetHashCode(trie.Right));
				hashcode = Utility.CreateHashCode(hashcode, trie.Letter);
				hashcode = Utility.CreateHashCode(hashcode, trie.IsWord ? 0 : 1);
				trie.Hashcode = hashcode == 0 ? 0x12345678 : hashcode;
			}

			return trie.Hashcode;
		}

		public static DynamicTrie Read(string path)
		{
			var file = File.OpenText(path);
			DynamicTrie trie = null;
			while (true)
			{
				var line = file.ReadLine();
				if (line == null)
					return trie;
				Insert(ref trie, line);
			}
		}

		public static TrieData Compress(ref DynamicTrie t)
		{
			var hash = new Dictionary<DynamicTrie, TrieData>();
			var result = Compress(ref t, hash);
			foreach (var v in hash.Values)
			{
				v.Left = Request(v.Trie.Left, hash);
				v.Right = Request(v.Trie.Right, hash);
				v.Middle = Request(v.Trie.Middle, hash);
			}
			return result;
		}

		public static TrieData Request(DynamicTrie t, Dictionary<DynamicTrie, TrieData> hash)
		{
			if (t == null)
				return null;
			var result = hash.Get(t);
			return result;
		}

		public static TrieData Compress(ref DynamicTrie t, Dictionary<DynamicTrie, TrieData> hash)
		{
			if (t == null)
				return NullTrie;

			var data = hash.Get(t);
			if (data != null)
			{
				Utility.Equals(ref data.Trie, ref t);
				data.Instances++;
				return data;
			}

			data = new TrieData {Trie = t, Instances = 1};
			hash.Set(t, data);

			var dataLeft = Compress(ref t.Left, hash);
			var dataRight = Compress(ref t.Right, hash);
			var dataMiddle = Compress(ref t.Middle, hash);

			data.CountChildren = dataMiddle.Count;
			data.CountSiblings = 1 + dataLeft.CountSiblings + dataRight.CountSiblings;
			return data;
		}

		public class TrieData
		{
			public int CountChildren;
			public int CountSiblings;
			public int Index;
			public int Instances;
			public TrieData Left;
			public TrieData Middle;
			public TrieData Right;
			public DynamicTrie Trie;

			public int Count
			{
				get { return CountSiblings + CountChildren; }
			}
		}
	}
}