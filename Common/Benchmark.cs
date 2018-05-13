using System.Diagnostics;

namespace Common
{
	internal static class Benchmark
	{
		public static int[][] JaggedArrayCreate()
		{
			var a = new int[9][];
			for (var i = 0; i < 9; ++i)
			{
				a[i] = new int[9];
			}

			return a;
		}

		public static void JaggedArrayWrite(ref int[][] a)
		{
			for (var i = 0; i < 9; ++i)
			{
				for (var j = 0; j < 9; ++j)
				{
					a[i][j] = 1;
				}
			}
		}

		public static int[,] MultidimensionalArrayCreate()
		{
			var b = new int[9, 9];
			return b;
		}

		public static void MultidimensionalArrayWrite(ref int[,] b)
		{
			for (var i = 0; i < 9; ++i)
			{
				for (var j = 0; j < 9; ++j)
				{
					b[i, j] = 1;
				}
			}
		}

		public delegate void VoidMethod();

		public static double SW(VoidMethod method)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			method();
			stopwatch.Stop();
			return stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;
		}
	}
}
