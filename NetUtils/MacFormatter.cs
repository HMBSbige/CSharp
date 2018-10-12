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
	}
}
