using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softperson;

namespace Softperson.Algorithms.Strings
{

    public class EditDistanceAlgo
    {
        // TODO: Reduce space of EditDistance to 2*min(m,n)
        // If only the length of the LCS is required, the matrix can be reduced to a 2\times
        // \min(n,m) matrix with ease, or to a \min(m,n)+1 vector (smarter) as the dynamic
        // programming approach only needs the current and previous columns of the matrix.
        // Hirschberg's algorithm allows the construction of the optimal sequence itself in
        // the same quadratic time and linear space bounds.[6]

        [DebuggerStepThrough]
        public static int EditDistanceSlow(string source, string target)
        {
            if (source.Length > target.Length)
                Utility.Swap(ref source, ref target);

            // Get Lengths
            if (source.Length == 0) return target.Length;

            // Initialize array
            var distance = new short[source.Length + 1, target.Length + 1];
            for (int i = 0; i <= source.Length; i++)
                distance[i, 0] = (short) i;
            for (int j = 0; j <= target.Length; j++)
                distance[0, j] = (short) j;

            // Perform edit distance tests
            for (int i = 0; i < source.Length; i++)
            {
                char ch1 = source[i];
                for (int j = 0; j < target.Length; j++)
                {
                    int d = distance[i, j];
                    if (ch1 != target[j]) d += 1;
                    d = Math.Min(d, distance[i + 1, j] + 1); // deletion
                    d = Math.Min(d, distance[i, j + 1] + 1); // insertion
                    distance[i + 1, j + 1] = (short) d;
                }
            }
            return distance[source.Length, target.Length];
        }

        [DebuggerStepThrough]
        public static unsafe int EditDistanceFast(string source, string target)
        {
            // Mark sure target length is smaller,
            // so space is O( min(source.Length, target.Length) )

            if (source.Length < target.Length)
                Utility.Swap(ref source, ref target);

            int sourceLength = source.Length;
            int targetLength = target.Length;
            int sourceStart = 0;
            int targetStart = 0;

            // Trim common prefix
            int prefixLength = 0;
            while (prefixLength < targetLength && source[prefixLength] == target[prefixLength])
                prefixLength++;

            if (prefixLength > 0)
            {
                sourceStart += prefixLength;
                targetStart += prefixLength;
                targetLength -= prefixLength;
                sourceLength -= prefixLength;
            }

            // Trim common suffix
            while (targetLength > 0 && source[sourceLength - 1] == target[targetLength - 1])
            {
                sourceLength--;
                targetLength--;
            }

            // Get Lengths
            if (targetLength == 0) return sourceLength;

            var dist = stackalloc int[targetLength];

            // Initialize array
            for (int j = 0; j < targetLength; j++)
                dist[j] = j + 1;

            // Perform edit distance tests
            for (int i = 0; i < sourceLength; i++)
            {
                var distpp = i; // dist[i-1, j-1] at j=0
                var distnp = i + 1; // dist[i, j-1] at j=0

                char ch1 = source[sourceStart + i];
                for (int j = 0; j < targetLength; j++)
                {
                    int distpn = dist[j]; // dist[i-1, j]

                    int distnn = distpp;
                    if (ch1 != target[targetStart + j]) distnn++;
                    distnn = Math.Min(distnn, distnp + 1); // deletion
                    distnn = Math.Min(distnn, distpn + 1); // insertion

                    // Next loop
                    distpp = distpn;
                    distnp = distnn;
                    dist[j] = distnn; // dist[i, j]
                }
            }

            return dist[targetLength - 1];
        }

	    public static unsafe int EditDistance(string source, string target,
			int insertion=1, int deletion=1, int substitution=1)
	    {
		    // Mark sure target length is smaller,
		    // so space is O( min(source.Length, target.Length) )

		    if (source.Length < target.Length)
			    Utility.Swap(ref source, ref target);

		    int sourceLength = source.Length;
		    int targetLength = target.Length;

		    // Get Lengths
		    if (targetLength == 0) return sourceLength;

		    var dist = stackalloc int[targetLength];

		    // Initialize array
		    for (int j = 0; j < targetLength; j++)
			    dist[j] = j + 1;

		    // Perform edit distance tests
		    for (int i = 0; i < sourceLength; i++)
		    {
			    var distpp = i; // dist[i-1, j-1] at j=0
			    var distnp = i + 1; // dist[i, j-1] at j=0

			    char ch1 = source[i];
			    for (int j = 0; j < targetLength; j++)
			    {
				    int distpn = dist[j]; // dist[i-1, j]

				    int distnn = distpp;
				    if (ch1 != target[j]) distnn += substitution;
				    distnn = Math.Min(distnn, distnp + deletion); 
				    distnn = Math.Min(distnn, distpn + insertion); 

				    // Next loop
				    distpp = distpn;
				    distnp = distnn;
				    dist[j] = distnn; // dist[i, j]
			    }
		    }

		    return dist[targetLength - 1];
	    }

	}
}
