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
		private static void Main(string[] args)
		{
			//crc32('36448850')==crc32('325985907')==0xF596D61E
			//86821 14E54344
			//14740600 14E54344

			const string str = @"F596D61E";
			Console.WriteLine(BilibiliUID.GetUID_First(str));
			long[] ans = null;
			var time = Benchmark.SW(() =>
			{
				ans = BilibiliUID.GetUID_All(str, (long)1e10);
			});
			Console.WriteLine($@"{time}秒");//1000+
			foreach (var x in ans)
			{
				Console.WriteLine(x);
			}

			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
