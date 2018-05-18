using System;
using System.Diagnostics;
using Common;
using Microsoft.Win32;

namespace CSharp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var version = Reg.Exist(@"HKEY_LOCAL_MACHINE\SAM1");
			Console.WriteLine(version);
			Console.WriteLine(@"END OF FILE");
		}
	}
}
