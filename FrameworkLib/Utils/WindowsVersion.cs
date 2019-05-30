using System;

namespace FrameworkLib.Utils
{
	public static class WindowsVersion
	{
		/// <summary>
		/// 返回WinVersion对应的字符串
		/// </summary>
		/// <param name="version">enum WinVersion</param>
		/// <returns></returns>
		public static string ToVersionString(this WinVersion version)
		{
			switch (version)
			{
				case WinVersion.Unknown: return @"Unknown";
				case WinVersion.Win3: return @"Win 3.1";
				case WinVersion.WinCE: return @"Win CE";
				case WinVersion.Win95: return @"Win95";
				case WinVersion.Win98: return @"Win98";
				case WinVersion.WinMe: return @"WinMe";
				case WinVersion.NT3: return @"NT 3.51";
				case WinVersion.NT4: return @"NT 4.0";
				case WinVersion.Win2000: return @"Win2000";
				case WinVersion.WinXP: return @"WinXP";
				case WinVersion.Win2003: return @"Win2003";
				case WinVersion.Vista: return @"Vista/Win2008Server";
				case WinVersion.Win7: return @"Win7/Win2008Server R2";
				case WinVersion.Win8: return @"Win8/Win2012Server";
				case WinVersion.Win8point1: return @"Win8.1/Win2012Server R2";
				case WinVersion.Win10: return @"Windows 10";
				default:
					throw new ArgumentOutOfRangeException(nameof(version), version, null);
			}
		}

		/// <summary>
		/// WinVersion 枚举
		/// </summary>
		public enum WinVersion
		{
			Unknown,
			Win3,//Win 3.1
			WinCE,//Win CE
			Win95,//Win95
			Win98,//Win98
			WinMe,//WinMe
			NT3,//NT 3.51
			NT4,//NT 4.0
			Win2000,//Win2000
			WinXP,//WinXP
			Win2003,//Win2003
			Vista,//Vista/Win2008Server
			Win7,//Win7/Win2008Server R2
			Win8,//Win8/Win2012Server
			Win8point1,//Win8.1/Win2012Server R2
			Win10,//Windows 10
		}

		/// <summary>
		/// 返回当前运行OS的版本
		/// </summary>
		/// <returns></returns>
		public static WinVersion GetOSVersion()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
					return WinVersion.Win3;
				case PlatformID.Win32Windows:
					switch (Environment.OSVersion.Version.Minor)
					{
						case 0:
							return WinVersion.Win95;
						case 10:
							return WinVersion.Win98;
						case 90:
							return WinVersion.WinMe;
					}
					break;
				case PlatformID.Win32NT:
					switch (Environment.OSVersion.Version.Major)
					{
						case 3:
							return WinVersion.NT3;
						case 4:
							return WinVersion.NT4;
						case 5:
							switch (Environment.OSVersion.Version.Minor)
							{
								case 0:
									return WinVersion.Win2000;
								case 1:
									return WinVersion.WinXP;
								case 2:
									return WinVersion.Win2003;
							}
							break;
						case 6:
							switch (Environment.OSVersion.Version.Minor)
							{
								case 0:
									return WinVersion.Vista;
								case 1:
									return WinVersion.Win7;
								case 2:
									return WinVersion.Win8;
								case 3:
									return WinVersion.Win8point1;
							}
							break;
						case 10:
							//需要清单文件开启，否则会被误判成Win8
							return WinVersion.Win10;
					}
					break;
				case PlatformID.WinCE:
					return WinVersion.WinCE;
			}
			return WinVersion.Unknown;
		}

		/// <summary>
		/// 返回当前运行OS的版本号的主要和次要部分
		/// </summary>
		/// <returns></returns>
		public static string GetOSSimpleVersion()
		{
			var version = Environment.OSVersion.Version;
			return $@"{version.Major}.{version.Minor}";
		}

		/// <summary>
		/// 返回当前运行OS的版本号
		/// </summary>
		/// <returns></returns>
		public static Version GetOSCompleteVersion()
		{
			return Environment.OSVersion.Version;
		}
	}
}
