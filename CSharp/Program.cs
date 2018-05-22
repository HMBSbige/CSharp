using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Win32;

namespace CSharp
{
	internal static class Program
	{
		private static void PrintCollision(uint num)
		{
			var m = new Dictionary<uint, uint>();
			for (uint i = 0; num > 0; ++i)
			{
				var str = i.ToString(@"D");
				var crc32 = CRC32.Get(str);
				if (m.ContainsKey(crc32))
				{
					Console.WriteLine($@"{m[crc32]} {crc32:X}");
					Console.WriteLine($@"{i} {crc32:X}");
					Console.Write(Environment.NewLine);
					--num;
				}
				else
				{
					m.Add(crc32, i);
				}
			}
			Console.Write(Environment.NewLine);
		}

		private static void Main(string[] args)
		{
			//crc32('36448850')==crc32('325985907')==0xF596D61E
			//86821 14E54344
			//14740600 14E54344
			const long i = (long)BilibiliUID.Maxuid;
			const long j = (long)14740600;//14740600;

			Console.WriteLine(BilibiliUID.CheckAns(1842575));
			var time = Benchmark.SW(() =>
			{
				Parallel.For(0, j, (x, loopState) =>
				{
					var xx = Convert.ToUInt64(x);
					if (!BilibiliUID.CheckAns(xx))
					{
						Console.WriteLine($@"{xx:D}");
						loopState.Stop();
					}
				});
			});
			Console.WriteLine($@"{time}秒");

			const string str = @"6E6C315E";
			/*
			ConcurrentBag<ulong> ans = null;
			ulong? ans2 = null;
			var time = Benchmark.SW(() =>
			{
				ans = BilibiliUID.BruteforceParallel(str);
			});
			Console.WriteLine($@"{time}秒");
			foreach (var x in ans)
			{
				Console.WriteLine(x);
			}
			Console.WriteLine(Environment.NewLine);

			time = Benchmark.SW(() =>
			{
				ans2 = BilibiliUID.Getuid(str);
			});
			Console.WriteLine($@"{time}秒");
			Console.WriteLine(ans2 ?? 0);
			*/
			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
