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
		public static void Test(string hostname, int port)
		{
			var ip = NetTest.GetIP(hostname);
			var res = NetTest.IsPortOpen(ip, port);
			if (res == null)
			{
				Console.WriteLine($@"{hostname}:{port}:Closed");
			}
			else
			{
				Console.WriteLine($@"{hostname}:{port}:{res}ms");
			}
		}
		private static void Main(string[] args)
		{

			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
