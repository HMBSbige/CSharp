using System;
using System.Diagnostics;

namespace CSharp
{
	internal class Program
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

		private delegate void VoidMethod();

		private static double BenchMark(VoidMethod method)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			method();
			stopwatch.Stop();
			return stopwatch.ElapsedTicks / (double) Stopwatch.Frequency;
		}

		static void Main(string[] args)
		{
			var a = JaggedArrayCreate();
			var b = MultidimensionalArrayCreate();
			const int n = 10000000;
			//Jagged Arrays
			Console.WriteLine(@"交错数组创建：" + BenchMark(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					a = JaggedArrayCreate();
				}
			}));

			Console.WriteLine(@"交错数组写：" + BenchMark(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					JaggedArrayWrite(ref a);
				}
			}));

			//Multidimensional Arrays
			Console.WriteLine(@"二维数组创建：" + BenchMark(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					b = MultidimensionalArrayCreate();
				}
			}));

			Console.WriteLine(@"二维数组写：" + BenchMark(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					MultidimensionalArrayWrite(ref b);
				}
			}));
		}
	}
}
