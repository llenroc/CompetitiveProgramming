#include <cmath>
#include <cstdio>
#include <vector>
#include <iostream>
#include <algorithm>
#include <string.h>
using namespace std;

int table[101][101];
int dp[101][101] = {};

int maxsum(int pos, int level, int n)
{
    int sum = table[level][pos];
    if (level == n) return sum;
    if (dp[pos][level]!=0) return dp[pos][level]-1;
    
    int sum1 = maxsum(pos, level+1, n);
    int sum2 = maxsum(pos+1, level+1, n);
    
    sum = sum + (sum1>sum2 ? sum1 : sum2);
    dp[pos][level] = sum+1;
    return sum;
}

int main() {
    int tests;
    cin >> tests;
    for (int t = 0; t<tests; t++)
    {
        int n;
        cin >> n;
        for (int i=1; i<=n; i++)
            for (int j=1; j<=i; j++)
                cin >> table[i][j]; 

        for (int i=0; i<=n; i++)
            for (int j=0; j<=n; j++)
                dp[i][j]=0;
        
        int answer = maxsum(1, 1, n);
        cout << answer << endl;
    }
    
    return 0;
}



