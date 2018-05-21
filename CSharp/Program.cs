using System;
using System.Diagnostics;
using System.Text;
using Common;
using Microsoft.Win32;

namespace CSharp
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			while (true)
			{
				var str = Console.ReadLine();
				if (str == null)
				{
					break;
				}

				var arrayOfBytes = Encoding.UTF8.GetBytes(str);
				str = Crc32.Get(arrayOfBytes).ToString(@"X");
				Console.WriteLine(Crc32.Getuid(str));
				/*
                var wanted = Convert.ToUInt32(str, 16);
                for (var i = 0; i < 1e9; ++i)
                {
                    var arrayOfBytes = Encoding.UTF8.GetBytes(i.ToString(@"D"));
                    if (wanted == Crc32.Crc32.Get(arrayOfBytes))
                    {
                        Console.WriteLine(i);
                    }
                }
                Console.WriteLine(@"OVER" + Environment.NewLine + Environment.NewLine);

                /*var arrayOfBytes = Encoding.UTF8.GetBytes(str);
                Console.WriteLine(Crc32.Crc32.Get(arrayOfBytes).ToString(@"X"));*/
			}
			Console.WriteLine(@"END OF FILE");
		}
	}
}
