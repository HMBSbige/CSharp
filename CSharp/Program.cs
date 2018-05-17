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
			var version2 = WindowsVersion.GetOSVersion().ToVersionString();
			Console.WriteLine(version2);
			Console.WriteLine(Convert.ToDouble(WindowsVersion.GetOSSimpleVersion()));
			Console.WriteLine(@"END OF FILE");
		}
	}
}
