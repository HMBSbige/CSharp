using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrameworkLib.Utils
{
	public static class BilibiliUID
	{
		#region Data
		private static readonly uint[] RTable;
		private const long Maxuid = (long)1e10;
		#endregion

		#region Constructors
		static BilibiliUID()
		{
			RTable = new uint[256];

			for (uint i = 0; i < 256; ++i)
			{
				RTable[CRC32.MChecksumTable[i] >> 24] = i;
			}
		}
		#endregion

		#region private

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

		private static uint? DeepCheck(string i, IReadOnlyList<uint> index)
		{
			var hash = ~CRC32.Get(i);
			var tc = hash & 0xff ^ index[2];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			var ans = tc - 48;

			hash = CRC32.MChecksumTable[index[2]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[1];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			ans = ans * 10 + tc - 48;

			hash = CRC32.MChecksumTable[index[1]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[0];
			if (!(tc <= 57 && tc >= 48))
			{
				return null;
			}
			ans = ans * 10 + tc - 48;
			return ans;
		}

		private static bool Isthatuid(string str, long i)
		{
			return Convert.ToUInt32(str, 16) == CRC32.Get(i.ToString(@"D"));
		}

		private static long[] BruteforceParallel(string crc32, long min = 0, long max = Maxuid)
		{
			var res = new ConcurrentBag<long>();
			Parallel.For(min, max, (x, loopState) =>
			{
				if (Isthatuid(crc32, x))
				{
					res.Add(x);
				}
			});
			return res.ToArray();
		}

		private static long? Finduidlow(string str)
		{
			var ans = BruteforceParallel(str, 0, 1000);
			if (ans.Length == 0)
			{
				return null;
			}
			return ans[0];
		}

		private static long? Finduidhigh(string str, long max = Maxuid)
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
			for (long j = 0; j < max / 1000; ++j)
			{
				var lastindex = CRC32LastIndex(j.ToString());
				if (lastindex == index[3])
				{
					var deepCheckData = DeepCheck(j.ToString(), index);
					if (deepCheckData != null)
					{
						return j * 1000 + deepCheckData;
					}
				}
			}
			return null;
		}

		#endregion

		#region public

		public static long? GetUID_First(string crc32, long max = Maxuid)
		{
			return Finduidlow(crc32) ?? Finduidhigh(crc32, max);
		}

		public static long[] GetUID_All(string crc32, long max = Maxuid)
		{
			long[] res;
			var firstuid = GetUID_First(crc32, max);
			if (firstuid != null)
			{
				res = BruteforceParallel(crc32, firstuid.Value, max);
			}
			else
			{
				throw new Exception(@"Cannot find the first number!");
			}
			return res;
		}

		#endregion
	}
}
