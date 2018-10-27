using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Steam
{
	internal static class SteamClientHelper
	{
		public static string GetSteamConfigPath()
		{
			return Path.Combine(GetPathFromRegistry(), @"config\config.vdf");
		}

		public static bool DoesSteamConfigExist()
		{
			return File.Exists(GetSteamConfigPath());
		}

		public static IEnumerable<string> GetAllLibraries()
		{
			yield return GetPathFromRegistry();
			foreach (var library in VdfHelper.GetKeyPairs(File.ReadAllLines(GetSteamConfigPath()), @"BaseInstallFolder_").Select(libraryPath => libraryPath.Value))
			{
				yield return Path.GetFullPath(library);
			}
		}

		public static IEnumerable<string> GetAllGames(string libraryPath)
		{
			return Directory.GetFiles(Path.Combine(libraryPath, @"steamapps"), @"*.acf");
		}

		private static IEnumerable<string> GetAllWorkshop()
		{
			return GetAllLibraries().Select(lib => Path.Combine(lib, @"steamapps", @"workshop", @"content")).Where(Directory.Exists);
		}

		private static IEnumerable<int> GetAllAppidInWorkshop(string workshopPath)
		{
			var paths = Directory.GetDirectories(workshopPath);
			var list = new List<int>();
			foreach (var item in paths)
			{
				if (int.TryParse(Path.GetFileNameWithoutExtension(item), out var result))
				{
					list.Add(result);
				}
			}
			return list;
		}

		public static IEnumerable<KeyValuePair<int, string>> GetAllAppidInWorkshop()
		{
			return from dir in GetAllWorkshop() from appid in GetAllAppidInWorkshop(dir) select new KeyValuePair<int, string>(appid, dir);
		}

		public static bool IsSteamRunning()
		{
			return Process.GetProcessesByName(@"Steam").Length > 0;
		}

		public static uint? GetActiveUserSteamId3()
		{
			uint? steamId3 = null;
			try
			{
				using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam\ActiveProcess"))
				{
					var str = registryKey?.GetValue(@"ActiveUser").ToString();
					if (uint.TryParse(str, out var tempUint))
					{
						steamId3 = tempUint;
					}
				}
			}
			catch
			{
				// ignored
			}

			return steamId3;
		}

		private static string GetPathFromRegistry()
		{
			string path;
			try
			{
				using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
				{
					path = registryKey?.GetValue(@"SteamPath").ToString();
				}
			}
			catch
			{
				path = string.Empty;
			}
			return string.IsNullOrWhiteSpace(path) ? path : Path.GetFullPath(path);
		}
	}
}
