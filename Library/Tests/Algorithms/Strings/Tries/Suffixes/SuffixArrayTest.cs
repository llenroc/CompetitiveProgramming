﻿#region Copyright
/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2005-2016, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////
#endregion

using NUnit.Framework;
using Softperson.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Softperson.Collections;
using static NUnit.Framework.Assert;


namespace Softperson.Algorithms.Strings
{
	
	[TestFixture]
	public class SuffixArrayTest : Test
	{
	
		#region Tests
		[Test]
		public static void Suffixes()
		{
			var tree = new SuffixArray("bobocel");
			var v = tree.Suffixes;

			AreEqual(7, v.Length);
			AreEqual(0, v[0]);
			AreEqual(1, v[5]);
			AreEqual(2, v[1]);
			AreEqual(3, v[6]);
			AreEqual(4, v[2]);
			AreEqual(5, v[3]);
			AreEqual(6, v[4]);
		}

		/// <summary>
		/// Test for Indices
		/// </summary>
		[Test]
		public void IndicesTest()
		{
			var tree = new SuffixArray("bobocel");
			var v = tree.Ranks;

			AreEqual(7, v.Length);
			AreEqual(0, v[0]);
			AreEqual(5, v[1]);
			AreEqual(1, v[2]);
			AreEqual(6, v[3]);
			AreEqual(2, v[4]);
			AreEqual(3, v[5]);
			AreEqual(4, v[6]);
		}
		
		[Test]
		public void SuffixAutomationIndicesTest()
		{
			var tree = new SuffixAutomaton("bobocel");
			var suffixArray = new SuffixArray(tree);
			var indices = suffixArray.Ranks;

			AreEqual(7, indices.Length);
			AreEqual(0, indices[0]);
			AreEqual(5, indices[1]);
			AreEqual(1, indices[2]);
			AreEqual(6, indices[3]);
			AreEqual(2, indices[4]);
			AreEqual(3, indices[5]);
			AreEqual(4, indices[6]);
		}

		[Test]
		public static void SuffixAutomatonSuffixesTest()
		{
			var tree = new SuffixAutomaton("bobocel");
			var suffixArray = new SuffixArray(tree);
			var v = suffixArray.Suffixes;

			AreEqual(7, v.Length);
			AreEqual(0, v[0]);
			AreEqual(1, v[5]);
			AreEqual(2, v[1]);
			AreEqual(3, v[6]);
			AreEqual(4, v[2]);
			AreEqual(5, v[3]);
			AreEqual(6, v[4]);
		}

		[Test]
		public void SuffixAutomatonConstruction()
		{
			void Check(string s)
			{
				var tree = new SuffixAutomaton(s);
				var suffixArray = new SuffixArray(tree);
				CheckSuffixArray(s, suffixArray);
			}

			Check("banana");
			Check("GEEKSFORGEEKS");
			Check("AAAAAAAAAA");
			Check("ABCDEFG");
			Check("ABABABA");
			Check("abcabxabcd");
			Check("CCAAACCCGATTA");
		}

		[Test]
		public void ClassicConstruction()
		{
			void Check(string s)
			{
				var suffixArray = new SuffixArray(s);
				CheckSuffixArray(s, suffixArray);
			}

			Check("banana");
			Check("GEEKSFORGEEKS");
			Check("AAAAAAAAAA");
			Check("ABCDEFG");
			Check("ABABABA");
			Check("abcabxabcd");
			Check("CCAAACCCGATTA");
		}

		[Test]
		public static void LongestCommonPrefixOfStringTest()
		{
			var suffix = new SuffixArray("bobocel");
			suffix.InitializeRMQ();
			AreEqual(2, suffix.LongestCommonPrefixOfString(0, 2));
			AreEqual(1, suffix.LongestCommonPrefixOfString(1, 3));
			AreEqual(0, suffix.LongestCommonPrefixOfString(0, 6));
			AreEqual(6, suffix.LongestCommonPrefixOfString(1, 1));
		}

		[Test]
		public static void LongestCommonPrefixOfLcpArrayTest()
		{
			var suffix = new SuffixArray("bobocel");
			suffix.InitializeRMQ();
			AreEqual(2, suffix.LongestCommonPrefixOfLcpArray(0, 1));
			AreEqual(1, suffix.LongestCommonPrefixOfLcpArray(5, 6));
			AreEqual(0, suffix.LongestCommonPrefixOfLcpArray(0, 6));
		}

		[Test]
		public static void LcpArrayTest()
		{
			var s = "bobocel";
			var tree = new SuffixArray(s);
			var sa = tree.Suffixes;
			var lcps = tree.LcpArray;
			var previous = s.Substring(sa[0]);
			for (int i = 1; i < s.Length; i++)
			{
				var current = s.Substring(sa[i]);
				var lcp = StringTools.LongestCommonPrefix(previous, current);
				AreEqual(lcp, lcps[i]);
				previous = current;
			}
		}

		static void CheckSuffixArray(string s, SuffixArray suffixArray0)
		{
			var suffixes = suffixArray0.Suffixes;
			AreEqual(s.Length, suffixes.Length);
			var prev = s.Substring(suffixes[0]);
			for (int i = 1; i < suffixes.Length; i++)
			{
				var cur = s.Substring(suffixes[i]);
				IsTrue(String.CompareOrdinal(prev, cur) <= 0);
				prev = cur;
			}
		}

		[Test]
		public static void CompareAlgorithmsTest()
		{
			int n = 200000;
			var sb = new StringBuilder(n);
			for (int i = 0; i < n && sb.Length + 6 < n; i++)
				sb.Append(i * 7);

			var s = sb.ToString();
			var st = new SuffixAutomaton(s);
			var sa = new SuffixArray(st);
			var suffixes = sa.Suffixes;
			var indices = sa.Ranks;
			var lcp = sa.LcpArray;


			var sa2 = new SuffixArray(s);
			var suffixes2 = sa2.Suffixes;
			var indices2 = sa2.Ranks;
			var lcp2 = sa2.LcpArray;

			IsTrue(ArrayTools.ArrayEqual(suffixes, suffixes2));
			IsTrue(ArrayTools.ArrayEqual(indices, indices2));
			IsTrue(ArrayTools.ArrayEqual(lcp, lcp2));
		}

		[Test]
		public static void CompareSuffixTreeAlgorithmsTest()
		{
			int n = 200000;
			var sb = new StringBuilder(n);
			for (int i = 0; i < n && sb.Length + 6 < n; i++)
				sb.Append(i * 7);

			var s = sb.ToString();
			var st = new SuffixTree(s+ "$");
			var sa = new SuffixArray(st.GetSuffixArray()) {String = s};
			var suffixes = sa.Suffixes;
			var indices = sa.Ranks;
			var lcp = sa.LcpArray;


			var sa2 = new SuffixArray(s);
			var suffixes2 = sa2.Suffixes;
			var indices2 = sa2.Ranks;
			var lcp2 = sa2.LcpArray;

			IsTrue(ArrayTools.ArrayEqual(suffixes, suffixes2));
			IsTrue(ArrayTools.ArrayEqual(indices, indices2));
			IsTrue(ArrayTools.ArrayEqual(lcp, lcp2));
		}

		[Test]
		//[Ignore("Performance")]
		public static void Performance()
		{
			int n = 1200000;
			var sb = new StringBuilder(n);
			for (int i = 0; i < n && sb.Length < n; i++)
				sb.Append(i * 7);

			var s = sb.ToString();

			int[] suffixes, indices, lcp;
			SuffixArray sa;

			InitPerformance();

			Restart();
			var st = new SuffixAutomaton(s);
			sa = new SuffixArray(st);
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Automaton Creation:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = false;
			SuffixArray.ClassicBuilder.UseQuickSort = false;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = true;
			SuffixArray.ClassicBuilder.UseQuickSort = false;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / Late Quit:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = false;
			SuffixArray.ClassicBuilder.UseQuickSort = true;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / QuickSort:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = true;
			SuffixArray.ClassicBuilder.UseQuickSort = true;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / QuickSort / Late Quit:\n " + Elapsed);

			//

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = true;
			SuffixArray.ClassicBuilder.UseQuickSort = false;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / Late Quit:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = false;
			SuffixArray.ClassicBuilder.UseQuickSort = false;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = true;
			SuffixArray.ClassicBuilder.UseQuickSort = true;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / QuickSort / Late Quit:\n " + Elapsed);

			Restart();
			sa = new SuffixArray(s);
			SuffixArray.ClassicBuilder.LateQuit = false;
			SuffixArray.ClassicBuilder.UseQuickSort = true;
			suffixes = sa.Suffixes;
			indices = sa.Ranks;
			lcp = sa.LcpArray;
			Console.WriteLine("\nSuffix Array Creation / QuickSort:\n " + Elapsed);

		}

		static TimeSpan startTime;
		static Process process;
		static Stopwatch watch;

		static void InitPerformance()
		{
			process = Process.GetCurrentProcess();
			watch = new Stopwatch();
		}

		static void Restart()
		{
			GC.Collect();
			startTime = process.UserProcessorTime;
			watch.Restart();
		}

		static TimeSpan Elapsed => watch.Elapsed; //process.UserProcessorTime - startTime;

		#endregion
	}
}