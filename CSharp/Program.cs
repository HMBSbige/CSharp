using System;
using System.Diagnostics;

namespace CSharp
{
	static class Program
	{
		private static int[][] JaggedArrayCreate()
		{
			var a = new int[9][];
			for (var i = 0; i < 9; ++i)
			{
				a[i] = new int[9];
			}
			return a;
		}

		private static void JaggedArrayWrite(ref int[][] a)
		{
			for (var i = 0; i < 9; ++i)
			{
				for (var j = 0; j < 9; ++j)
				{
					a[i][j] = 1;
				}
			}
		}

		private static int[,] MultidimensionalArrayCreate()
		{
			var b = new int[9, 9];
			return b;
		}

		private static void MultidimensionalArrayWrite(ref int[,] b)
		{
			for (var i = 0; i < 9; ++i)
			{
				for (var j = 0; j < 9; ++j)
				{
					b[i, j] = 1;
				}
			}
		}

		static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();
			int[][] a = JaggedArrayCreate();
			int[,] b = MultidimensionalArrayCreate();
			const int n = 1000000;
			//Jagged Arrays
			stopwatch.Start();
			for (var i = 0; i < n; ++i)
			{
				a = JaggedArrayCreate();
			}
			stopwatch.Stop();
			Console.WriteLine(@"交错数组创建：" + stopwatch.ElapsedTicks);

			stopwatch.Reset();
			stopwatch.Start();
			for (var i = 0; i < n; ++i)
			{
				JaggedArrayWrite(ref a);
			}
			stopwatch.Stop();
			Console.WriteLine(@"交错数组写：" + stopwatch.ElapsedTicks);

			//Multidimensional Arrays
			stopwatch.Reset();
			stopwatch.Start();
			for (var i = 0; i < n; ++i)
			{
				b = MultidimensionalArrayCreate();
			}
			stopwatch.Stop();
			Console.WriteLine(@"二维数组创建：" + stopwatch.ElapsedTicks);

			stopwatch.Reset();
			stopwatch.Start();
			for (var i = 0; i < n; ++i)
			{
				MultidimensionalArrayWrite(ref b);
			}
			stopwatch.Stop();
			Console.WriteLine(@"二维数组写：" + stopwatch.ElapsedTicks);
		}
	}
}
