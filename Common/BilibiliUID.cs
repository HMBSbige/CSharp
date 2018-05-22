using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
	public static class BilibiliUID
	{
		#region TobilibiliUID

		private static readonly uint[] RTable;
		public const ulong Maxuid = (ulong)1e8;

		static BilibiliUID()
		{
			RTable = new uint[256];

			for (uint i = 0; i < 256; ++i)
			{
				RTable[CRC32.MChecksumTable[i] >> 24] = i;
			}
		}

		private static uint CRC32LastIndex(string str)
		{
			var crcstart = 0xFFFFFFFF;
			var len = str.Length;
			uint index = 0;
			for (var i = 0; i < len; ++i)
			{
				index = (crcstart ^ str[i]) & 0xFF;
				crcstart = (crcstart >> 8) ^ CRC32.MChecksumTable[index];
			}
			return index;
		}

		private static uint Getcrcindex(uint t)
		{
			return RTable[t];
		}

		private static string DeepCheck(string i, IReadOnlyList<uint> index)
		{
			var hash = ~CRC32.Get(i);
			var tc = hash & 0xff ^ index[2];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			var a1 = tc - 48;

			hash = CRC32.MChecksumTable[index[2]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[1];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			var a2 = tc - 48;

			hash = CRC32.MChecksumTable[index[1]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[0];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			var a3 = tc - 48;

			return $@"{a1}{a2}{a3}";
		}

		private static bool Isthatuid(string str, ulong i)
		{
			return Convert.ToUInt32(str, 16) == CRC32.Get(i.ToString(@"D"));
		}

		public static ConcurrentBag<ulong> BruteforceParallel(string str, long min = 0, long max = (long)Maxuid)
		{
			var res = new ConcurrentBag<ulong>();
			Parallel.For(min, max, (x, loopState) =>
			{
				if (Isthatuid(str, (ulong)x))
				{
					res.Add((ulong)x);
				}
			});
			return res;
		}

		private static ulong? Bruteforce(string str, ulong min, ulong max)
		{
			for (var i = min; i <= max; ++i)
			{
				if (Isthatuid(str, i))
				{
					return i;
				}
			}
			return null;
		}

		private static ulong? Finduidlow(string str)
		{
			return Bruteforce(str, 0, 999);
		}

		private static ulong? Finduidhigh(string str, ulong max = Maxuid)
		{
			var index = new uint[4];
			var ht = Convert.ToUInt32(str, 16) ^ 0xffffffff;
			int i;
			for (i = 3; i >= 0; --i)
			{
				index[3 - i] = Getcrcindex(Convert.ToUInt32(ht >> (i * 8)));
				var snum = CRC32.MChecksumTable[index[3 - i]];
				ht ^= snum >> ((3 - i) * 8);
			}
			for (ulong j = 0; j < max / 1000; ++j)
			{
				var lastindex = CRC32LastIndex(j.ToString());
				if (lastindex == index[3])
				{
					var deepCheckData = DeepCheck(j.ToString(), index);
					if (deepCheckData != null)
					{
						return Convert.ToUInt64(j + deepCheckData);
					}
				}
			}
			return null;
		}

		public static ulong? Getuid(string str)
		{
			return Finduidlow(str) ?? Finduidhigh(str);
		}

		public static bool CheckAns(ulong i)
		{
			var crc32 = CRC32.Get(i.ToString(@"D"));
			var rcrc32 = Getuid(crc32.ToString(@"X"));
			return rcrc32 == i;
		}

		public static bool CheckOne(ulong i)
		{
			var crc32 = CRC32.Get(i.ToString(@"D"));
			var rcrc32 = Getuid(crc32.ToString(@"X"));
			if (rcrc32 != null)
			{
				var r = CRC32.Get(rcrc32.Value.ToString(@"D"));
				return r == crc32;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
