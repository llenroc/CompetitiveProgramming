using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    static int[] h = new int[9];
    static int[] v = new int[9];
    static int[] r = new int[9];
    static char[][] s = new char[9][];
    static int zeroes;
    
    static void Main(String[] args) {

        for(int i=0; i<9; i++)
            s[i] = Console.ReadLine().ToCharArray();
        
        for (int x=0; x<9; x++)
        for (int y=0; y<9; y++)
        {
            var c = s[x][y] - '0';
            if (c == 0) { zeroes++; continue; }
            h[x] |= 1<<c;
            v[y] |= 1<<c;
            r[(x/3)*3+(y/3)] |= 1<<c;
        }

        SolveSudoku(s);
        
        foreach(var line in s)
            Console.WriteLine(new string(line));
    
    }
    
    public static  bool Solve()
    {
        while (zeroes > 0)
        {
            bool found = false;
            bool progress = false;
            for (int x=0; x<9; x++)
            for (int y=0; y<9; y++)
            {
                var c = s[x][y] - '0';
                if (c != 0) { continue; }

                found = true;
                var ss = (x/3)*3+(y/3);
                var fl = (1<<10) - 2;
                
                fl &= ~(h[x] | v[y] | r[ss]);
   
                if (fl==0) return false;
                
                if ((fl&fl-1) != 0) continue;
                
                int b = 0;
                while (fl > 0)
                {
                    fl >>= 1;
                    b++;
                }

                b--;
                s[x][y] = (char)('0' + b);
                
                h[x] |= 1<<b;
                v[y] |= 1<<b;
                r[ss] |= 1<<b;
                zeroes--;
                progress = true;
            }
            
           if (!progress) break;
        }

        if (zeroes == 0) return true;
        
        for (int x=0; x<9; x++)
            for (int y=0; y<9; y++)
            {
                var c = s[x][y] - '0';
                if (c != 0) { continue; }

                var ss = (x/3)*3+(y/3);
                var fl = (1<<10) - 2;

                fl &= ~(h[x] | v[y] | r[ss]);
                if (fl==0) return false;

                for (int b = 1; b<=9; b++)
                {
                    var bit = 1<<b;
                    if ((fl & bit) == 0) continue;
                    
                    var ph = h[x];
                    var pv = v[y];
                    var pr = r[ss];
                    var z = zeroes;

                    int[] hClone = (int[]) h.Clone();
                    int[] vClone = (int[]) v.Clone();
                    int[] rClone = (int[]) r.Clone();
                    var sclone = CloneMatrix(s);
                    
                    h[x] |= bit;
                    v[y] |= bit;
                    r[ss] |= bit;
                    s[x][y] = (char)('0' + b);
                    
                    zeroes--;
                    
                    if (Solve()) return true;
                    
                    zeroes = z;
                    h = hClone;
                    v = vClone;
                    r = rClone;
                    s = sclone;
                }
            }
        
        return false;
    }
    
    static char[][] CloneMatrix(char[][] orig)
    {
        var c = (char[][]) orig.Clone();
        for(int i=0; i<9; i++)
            c[i] = (char[]) c[i].Clone();
        return c;
    }
    
    public static void SolveSudoku(char[][] board) {
        Dfs(board, 0, 0);
    }
    
    static bool Dfs(char[][] board, int r, int c)
    {
        int rows = 9;
        int cols = 9;
        
        while (r<rows && c<cols && board[r][c] != '0')
        {
            c++;
            if (c>=cols) { c = 0; r++; }
        }
        
        if (r>=rows)
            return true;
        
        int validnums = ValidNumbers(board,r,c);
        for (int i=0; i<9; i++)
        {
            if ((validnums & (1<<i)) != 0)
            {
                board[r][c] = (char)('1' + i);
                if (Dfs(board, r,c))
                    return true;
            }
        }

        board[r][c] = '0';
        return false;
    }
    
    public static int ValidNumbers(char[][] board, int r, int c)
    {
        int mask = 0;
        
        int rs = (r/3)*3;
        int cs = (c/3)*3;
        for (int i=0; i<9; i++)
        {
            if (board[i][c] != '0') mask |= 1<<(board[i][c]-'1');
            if (board[r][i] != '0') mask |= 1<<(board[r][i]-'1');
            if (board[rs+i/3][cs+i%3] != '0') mask |= 1<<(board[rs+i/3][cs+i%3]-'1');
        }
        
        return mask ^ (1<<9)-1;
    }
    
}