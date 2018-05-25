using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Common;

namespace CSharp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			WinProcess.Stop(@"explorer");
			Console.WriteLine(Environment.NewLine + @"END OF FILE");
			Console.Read();
		}
	}
}
