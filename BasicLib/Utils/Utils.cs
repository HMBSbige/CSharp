using System;

namespace BasicLib.Utils
{
	public static class Utils
	{
		public static void OutputBytes(byte[] b)
		{
			if (b != null)
			{
				Console.WriteLine(BitConverter.ToString(b).Replace("-", string.Empty));
			}
			else
			{
				Console.WriteLine();
			}
		}
	}
}
