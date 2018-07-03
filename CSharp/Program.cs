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
		public static async void Test(string hostname, int port)
		{
			var ip = await NetTest.GetIP(hostname);
			var res = await NetTest.IsPortOpen(ip, port);
			if (res == null)
			{
				Console.WriteLine($@"{hostname}:{port}:Closed");
			}
			else
			{
				Console.WriteLine($@"{hostname}:{port}:{res}ms");
			}
		}

		private static async void OutputPublicIP()
		{
			var ip = await NetTest.GetPublicIpAddress();
			Console.WriteLine($@"Public IP address is: {ip}");
		}
		private static void Main(string[] args)
		{
			OutputPublicIP();
			Test(@"asf.bige0.vip", 443);
			Test(@"www.baidu.com", 443);

			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
