using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SOURCE: https://www.topcoder.com/community/data-science/data-science-tutorials/maximum-flow-augmenting-path-algorithms-comparison/

namespace Softperson.Algorithms.Graphs.NetworkFlow
{
	public class OptimizedMaxFlow
	{
		const int oo = 1000000000; // Infinity

		// Nodes, Arcs, the source node and the sink node
		int n, m, source, sink;

		// Matrixes for maintaining
		// Graph and Flow
		int[,] G;
		int[,] F;

		int[] pi; // predecessor list
		int[] CurrentNode; // Current edge for each node

		int[] queue; // Queue for reverse BFS

		int[] d; // Distance function
		int[] numbs; // numbs[k] is the number of nodes i with d[i]==k

		// Reverse breadth-first search
		// to establish distance function d

		public OptimizedMaxFlow(int N)
		{
			this.n = N;
			G = new int[n, n];
			F = new int[n, n];
			pi = new int[n];
			CurrentNode = new int[n];
			queue = new int[n];
			d = new int[n];
			numbs = new int[n];

		}


		int Rev_BFS()
		{
			int i, j;
			int head = 0;
			int tail = 0;

			// Initially, all d[i]=n
			for (i = 1; i <= n; i++)
				numbs[d[i] = n]++;

			// Start from the sink
			numbs[n]--;
			d[sink] = 0;
			numbs[0]++;

			queue[++tail] = sink;

			// While queue is not empty
			while (head != tail)
			{
				i = queue[++head]; // Get the next node

				// Check all adjacent nodes
				for (j = 1; j <= n; j++)
				{

					// If it was reached before or there is no edge
					// then continue
					if (d[j] < n || G[j,i] == 0) continue;

					// j is reached first time
					// put it into queue
					queue[++tail] = j;

					// Update distance function
					numbs[n]--;
					d[j] = d[i] + 1;
					numbs[d[j]]++;

				}
			}

			return 0;
		}

		// Augmenting the flow using predecessor list pi[]
		int Augment()
		{
			int i, j;
			int tmp;
			int width = (oo);

			// Find the capacity of the path
			for (i = sink, j = pi[i]; i != source; i = j, j = pi[j])
			{
				tmp = G[j,i];
				if (tmp < width) width = tmp;
			}

			// Augmentation itself
			for (i = sink, j = pi[i]; i != source; i = j, j = pi[j])
			{
				G[j,i] -= width; F[j,i] += width;
				G[i,j] += width; F[i,j] -= width;
			}

			return width;
		}

		// Relabel and backtrack
		int Retreat(ref int i)
		{
			int tmp;
			int j, mind = (n - 1);

			// Check all adjacent edges
			// to find nearest
			for (j = 1; j <= n; j++)
				// If there is an arc
				// and j is "nearer"
				if (G[i,j] > 0 && d[j] < mind)
					mind = d[j];

			tmp = d[i]; // Save previous distance

			// Relabel procedure itself 
			numbs[d[i]]--;
			d[i] = 1 + mind;
			numbs[d[i]]++;

			// Backtrack, if possible (i is not a local variable! )
			if (i != source) i = pi[i];

			// If numbs[ tmp ] is zero, algorithm will stop
			return numbs[tmp];
		}

		// Main procedure
		int Find_max_flow()
		{
			int flow = (0);
			int i, j;

			Rev_BFS(); // Establish exact distance function

			// For each node current arc is the first arc
			for (i = 1; i <= n; i++) CurrentNode[i] = 1;

			// Begin searching from the source
			i = source;

			// The main cycle (while the source is not "far" from the sink)
			for (; d[source] < n;)
			{

				// Start searching an admissible arc from the current arc
				for (j = CurrentNode[i]; j <= n; j++)
					// If the arc exists in the residual network
					// and if it is an admissible
					if (G[i,j] > 0 && d[i] == d[j] + 1)
						// Then finish searhing
						break;

				// If the admissible arc is found
				if (j <= n)
				{
					CurrentNode[i] = j; // Mark the arc as "current"
					pi[j] = i; // j is reachable from i 
					i = j; // Go forward

					// If we found an augmenting path
					if (i == sink)
					{
						flow += Augment(); // Augment the flow
						i = source; // Begin from the source again
					}
				}
				// If no an admissible arc found
				else
				{
					CurrentNode[i] = 1; // Current arc is the first arc again

					// If numbs[ d[i] ] == 0 then the flow is the maximal
					if (Retreat(ref i) == 0)
						break;

				}

			} // End of the main cycle

			// We return flow value
			return flow;
		}


		// The main function
		// Graph is represented in input as triples 

		// No comments here
		int Main()
		{
			int i, p, q, r;

			var input = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
			n = input[0];
			m = input[1];
			source = input[2];
			sink = input[3];

			for (i = 0; i < m; i++)
			{
				input = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
				p = input[0];
				q = input[1];
				r = input[2];
				G[p,q] += r;
			}

			Console.WriteLine("%d", Find_max_flow());
			return 0;
		}
	}
}
