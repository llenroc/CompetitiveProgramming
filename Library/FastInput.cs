using System;

public static class FastInput
{
    static System.IO.Stream stream;
    static int idx,  bytesRead;
    static byte[] buffer;
    static System.Text.StringBuilder builder;
    const int MonoBufferSize = 4096;


    public static void InitIO(
        int stringCapacity = 16,
        System.IO.Stream input = null)
    {
        builder = new System.Text.StringBuilder(stringCapacity);
        stream = input ?? Console.OpenStandardInput();
        idx = bytesRead = 0;
        buffer = new byte[MonoBufferSize];
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
		if (neg) { c = Read(); }

		int number = c - '0';
		while (true)
		{
			var d = Read() - '0';
			if ((uint)d > 9) break;
			number = number * 10 + d;
			if (number < 0) throw new FormatException();
		}
		return neg ? -number : number;
	}

	public static long Nl()
	{
		var c = SkipSpaces();
		bool neg = c == '-';
		if (neg) { c = Read(); }

		long number = c - '0';
		while (true)
		{
			var d = Read() - '0';
			if ((uint)d > 9) break;
			number = number * 10 + d;
			if (number < 0) throw new FormatException();
		}
		return neg ? -number : number;
	}


	public static char[] Nc(int n)
    {
        var list = new char[n];
        for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (char)c;
        return list;
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
        do  c = Read(); while ((uint)c - 33 >= (127 - 33));
        return c;
    }
}
