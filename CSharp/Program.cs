using Common;
using Steam;
using System;
using System.IO;

namespace CSharp
{
	class Program
	{
		public static DateTime GetTime(string timeStamp)
		{
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long lTime = long.Parse(timeStamp + "0000000");
			TimeSpan toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}

		private static void Main(string[] args)
		{
			Console.WriteLine($@"{SteamClientHelper.GetActiveUserSteamId3()}");
			Console.WriteLine($@"{SteamClientHelper.IsSteamRunning()}");
			Console.WriteLine($@"{SteamClientHelper.GetExePath()}");
			Console.WriteLine($@"{SteamClientHelper.GetPath()}");
			Console.WriteLine($@"{SteamClientHelper.GetAutoLoginUser()}");
			Console.WriteLine(GetTime(@"1532239797"));
			SteamClientHelper.SetAutoLoginUser(@"hmbsbige");
			SteamClientHelper.SetRememberPassword(true);
			WinProcess.Restart(SteamClientHelper.GetExePath(), @"-dev");
			//Console.WriteLine(s);
			//Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
