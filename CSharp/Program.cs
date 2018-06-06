using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Common;

namespace CSharp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var ip = NetTest.GetIP(@"www.baidu.com");
			for (var i = 0; i < 10; ++i)
			{
				var ans = NetTest.TCPing(ip,80);
				Console.WriteLine(ans);
				Thread.Sleep(1000);
			}
			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
