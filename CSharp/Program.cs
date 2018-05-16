using System;
using System.Diagnostics;
using Common;
using Microsoft.Win32;

namespace CSharp
{
	internal class Program
	{
		static void Main(string[] args)
		{
			const string portPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
			Reg.Set(portPath, @"PortNumber", 3389, RegistryValueKind.DWord);
			Reg.Delete(portPath);
		}
	}
}
