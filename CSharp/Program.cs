using Common;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp
{
	class Program
	{
		public static string GetPublicIp(IPAddress ip)
		{
			return NetTest.GetPublicIpAddress().Result.ToString();
		}

		private static void Main(string[] args)
		{
			

			//Console.WriteLine(@"Aborting...");

			//Console.WriteLine(@"Aborted...");
			//Console.WriteLine(@"Stop");
			//Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
