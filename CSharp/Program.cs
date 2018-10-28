using Steam;
using System;

namespace CSharp
{
	class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine($@"{SteamClientHelper.GetActiveUserSteamId3()}");

			//Console.WriteLine(s);
			//Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
