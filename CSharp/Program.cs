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
			var version2 = Reg.Exist(@"计算机\HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", @"AUOptions");
			Console.WriteLine(version2);
			Console.WriteLine(@"END OF FILE");
		}
	}
}
