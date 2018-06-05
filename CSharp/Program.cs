using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
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
			var remote = new IPEndPoint(IPAddress.Parse(@""), 3389);
			var times = new List<double>();
			for (var i = 0; i < 4; ++i)
			{
				var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
				{
					Blocking = true
				};

				var stopwatch = new Stopwatch();

				stopwatch.Start();
				sock.Connect(remote);
				stopwatch.Stop();

				var t = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine("{0:0.00}ms", t);
				times.Add(t);

				sock.Close();

				Thread.Sleep(1000);
			}

			Console.WriteLine("{0:0.00} {1:0.00} {2:0.00}", times.Min(), times.Max(), times.Average());
			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
