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
			var ip = IPAddress.Parse(@"1.0.0.0");
			Console.WriteLine(IPv4Subnet.IPv42UintLE(ip));
			Console.WriteLine(IPv4Subnet.IPv42UintBE(ip));
			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
