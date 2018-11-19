using System.Text;
using System.Text.RegularExpressions;

namespace NetUtils
{
	public class MacFormatter
	{
		private static readonly Regex MacPattern = new Regex("[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]:[0-9A-F][0-9A-F]");

		public static bool CheckMac(string mac)
		{
			return MacPattern.IsMatch(mac);
		}

		/// <summary>
		/// 全小写MAC地址
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ShortMac(string s)
		{
			return !string.IsNullOrWhiteSpace(s) ? s.Replace(@"-", @"").Replace(@":", @"").ToLower() : string.Empty;
		}

		/// <summary>
		/// 格式化MAC地址（大写、':' 分割）
		/// </summary>
		/// <param name="s"></param>
		/// <param name="isUpper"></param>
		/// <returns></returns>
		public static string MacFormat(string s, bool isUpper = true)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(s))
			{
				if (s.Length == 12)
				{
					sb.Append(
							$@"{s.Substring(0, 2)}:" +
							$@"{s.Substring(2, 2)}:" +
							$@"{s.Substring(4, 2)}:" +
							$@"{s.Substring(6, 2)}:" +
							$@"{s.Substring(8, 2)}:" +
							$@"{s.Substring(10, 2)}");
				}
			}

			return isUpper ? sb.ToString().ToUpper() : sb.ToString().ToLower();
		}
	}
}
