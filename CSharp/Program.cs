using Common;
using Gameloop.Vdf;
using Steam;
using System;
using System.IO;
using Gameloop.Vdf.Linq;

namespace CSharp
{
	class Program
	{
		public static DateTime GetTime(string timeStamp)
		{
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long lTime = long.Parse($@"{timeStamp}0000000");
			TimeSpan toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}

		private static void SteamTest()
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
		}

		private static void Main(string[] args)
		{
			var str = File.ReadAllText(SteamClientHelper.GetLoginUsersConfigPath());
			//SteamTest();
			dynamic volvo = VdfConvert.Deserialize(str);
			VToken v2 = volvo.Value;
			foreach (var child in v2.Children())
			{
				Console.WriteLine($@"SteamID64:{child.Key}");
				Console.WriteLine($@"AccountName:{child.Value[@"AccountName"]}");
				Console.WriteLine($@"PersonaName:{child.Value[@"PersonaName"]}");
				Console.WriteLine($@"RememberPassword:{child.Value[@"RememberPassword"]}");
				Console.WriteLine($@"mostrecent:{child.Value[@"mostrecent"]}");
				Console.WriteLine($@"Timestamp:{child.Value[@"Timestamp"]}");
				Console.WriteLine();
			}
			

			//Console.WriteLine(s);
			//Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
