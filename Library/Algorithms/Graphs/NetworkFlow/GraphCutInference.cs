using System;
using System.Collections;
using System.Collections.Generic;

namespace Softperson.Algorithms.Graphs
{

	// Special-purpose {0,1} combinatorial optimization solver for
	// problems of the following by a reduction to graph cuts:
	//
	//        minimize         sum_i  psi_i(x[i]) 
	//  x[1]...x[n] in {0,1}      + sum_{i < j}  phi_{ij}(x[i], x[j])
	//
	// where
	//      psi_i : {0, 1} --> R
	//   phi_{ij} : {0, 1} x {0, 1} --> R
	//
	// such that
	//   phi_{ij}(0,0) + phi_{ij}(1,1) <= phi_{ij}(0,1) + phi_{ij}(1,0)  (*)
	//
	// This can also be used to solve maximization problems where the
	// direction of the inequality in (*) is reversed.
	//
	// INPUT: phi -- a matrix such that phi[i][j][u][v] = phi_{ij}(u, v)
	//        psi -- a matrix such that psi[i][u] = psi_i(u)
	//        x -- a List where the optimal solution will be stored
	//
	// OUTPUT: value of the optimal solution
	//
	// To use this code, create a GraphCutInference object, and call the
	// DoInference() method.  To perform maximization instead of minimization,
	// ensure that #define MAXIMIZATION is enabled.
	public class GraphCutInference
	{
		private const int Inf = 1000000000;

		private const bool Maximization = true;
		private List<List<int>> _cap;
		private List<List<int>> _flow;
		// comment out following line for minimization

		private int _n;
		private BitArray _reached;

		private int Augment(int s, int t, int a)
		{
			_reached[s] = true;
			if (s == t) return a;
			for (var k = 0; k < _n; k++)
			{
				if (_reached[k]) continue;
				var aa = Math.Min(a, _cap[s][k] - _flow[s][k]);
				if (aa != 0)
				{
					var b = Augment(k, t, aa);
					if (b != 0)
					{
						_flow[s][k] += b;
						_flow[k][s] -= b;
						return b;
					}
				}
			}
			return 0;
		}

		public int GetMaxFlow(int s, int t)
		{
			_n = _cap.Count;
			_flow = STL.Repeat(_n, STL.Repeat(_n, 0));
			_reached = new BitArray(_n);

			var totflow = 0;
			int amt;
			while ((amt = Augment(s, t, Inf)) != 0)
			{
				totflow += amt;
				_reached.SetAll(false);
			}
			return totflow;
		}

		public int DoInference(List<List<List<List<int>>>> phi, List<List<int>> psi, List<int> x)
		{
			var M = phi.Count;
			_cap = STL.Repeat(M + 2, STL.Repeat(M + 2, 0));
			var b = STL.Repeat<int>(M);
			var c = 0;

			for (var i = 0; i < M; i++)
			{
				b[i] += psi[i][1] - psi[i][0];
				c += psi[i][0];
				for (var j = 0; j < i; j++)
					b[i] += phi[i][j][1][1] - phi[i][j][0][1];
				for (var j = i + 1; j < M; j++)
				{
					_cap[i][j] = phi[i][j][0][1] + phi[i][j][1][0] - phi[i][j][0][0] - phi[i][j][1][1];
					b[i] += phi[i][j][1][0] - phi[i][j][0][0];
					c += phi[i][j][0][0];
				}
			}

			if (Maximization)
			{
				for (var i = 0; i < M; i++)
				{
					for (var j = i + 1; j < M; j++)
						_cap[i][j] *= -1;
					b[i] *= -1;
				}
				c *= -1;
			}

			for (var i = 0; i < M; i++)
			{
				if (b[i] >= 0)
				{
					_cap[M][i] = b[i];
				}
				else
				{
					_cap[i][M + 1] = -b[i];
					c += b[i];
				}
			}

			var score = GetMaxFlow(M, M + 1);
			_reached.SetAll(false);
			Augment(M, M + 1, Inf);
			x = new List<int>(M);
			for (var i = 0; i < M; i++)
				x[i] = _reached[i] ? 0 : 1;
			score += c;

			if (Maximization)
				score *= -1;

			return score;
		}

	}
}