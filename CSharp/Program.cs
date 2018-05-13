using System;
using System.Diagnostics;
using Common;

namespace CSharp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var a = Benchmark.JaggedArrayCreate();
			var b = Benchmark.MultidimensionalArrayCreate();
			const int n = 10000000;

			//Jagged Arrays
			Console.WriteLine(@"交错数组创建：" + Benchmark.SW(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					a = Benchmark.JaggedArrayCreate();
				}
			}));

			Console.WriteLine(@"交错数组写：" + Benchmark.SW(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					Benchmark.JaggedArrayWrite(ref a);
				}
			}));

			//Multidimensional Arrays
			Console.WriteLine(@"二维数组创建：" + Benchmark.SW(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					b = Benchmark.MultidimensionalArrayCreate();
				}
			}));

			Console.WriteLine(@"二维数组写：" + Benchmark.SW(() =>
			{
				for (var i = 0; i < n; ++i)
				{
					Benchmark.MultidimensionalArrayWrite(ref b);
				}
			}));
		}
	}
}
