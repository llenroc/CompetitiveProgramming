﻿using System.Collections;
using System.Collections.Generic;

namespace Softperson.Algorithms.Graphs
{
	public class SCC
	{
		readonly List<Edge> _e = new List<Edge>();
		readonly List<Edge> _er = new List<Edge>();
		readonly List<int> _groupNum = new List<int>();
		readonly List<int> _sp = new List<int>();
		readonly List<int> _spr = new List<int>();
		readonly List<int> _stk = new List<int>();
		readonly BitArray _v;
		int _groupCnt;

		readonly int _vertexCount;
		int _edgeCount;
		public int Maxe;
		public int Maxv;

		public SCC(int vertexCount)
		{
			_vertexCount = vertexCount;
			_v = new BitArray(_vertexCount);
		}

		private void FillForward(int x)
		{
			int i;
			_v[x] = true;
			for (i = _sp[x]; i != 0; i = _e[i].Nxt) if (!_v[_e[i].e]) FillForward(_e[i].e);
			_stk[++_stk[0]] = x;
		}

		private void FillBackward(int x)
		{
			int i;
			_v[x] = false;
			_groupNum[x] = _groupCnt;
			for (i = _spr[x]; i != 0; i = _er[i].Nxt) if (_v[_er[i].e]) FillBackward(_er[i].e);
		}

		public void AddEdge(int v1, int v2) //add edge v1->v2
		{
			++_edgeCount;
			_e[_edgeCount] = new Edge {e = v2, Nxt = _sp[v1]};
			_sp[v1] = _edgeCount;

			_er[_edgeCount] = new Edge {e = v1, Nxt = _spr[v2]};
			_spr[v2] = _edgeCount;
		}

		public void Run()
		{
			int i;
			_stk[0] = 0;
			_v.SetAll(false);
			for (i = 1; i <= _vertexCount; i++) if (!_v[i]) FillForward(i);
			_groupCnt = 0;
			for (i = _stk[0]; i >= 1; i--)
				if (_v[_stk[i]])
				{
					_groupCnt++;
					FillBackward(_stk[i]);
				}
		}

		public struct Edge
		{
			public int e;
			public int Nxt;
		}
	}
}