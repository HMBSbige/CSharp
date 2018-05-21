using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	/// <summary>
	/// Performs 32-bit reversed cyclic redundancy checks.
	/// </summary>
	public static class Crc32
	{
		#region Constants
		/// <summary>
		/// Generator polynomial (modulo 2) for the reversed CRC32 algorithm. 
		/// </summary>
		private const uint SGenerator = 0xEDB88320;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the CRC32 class.
		/// </summary>
		static Crc32()
		{
			// Constructs the checksum lookup table. Used to optimize the checksum.
			MChecksumTable = Enumerable.Range(0, 256).Select(i =>
			{
				var tableEntry = (uint)i;
				for (var j = 0; j < 8; ++j)
				{
					tableEntry = (tableEntry & 1) != 0
						? SGenerator ^ (tableEntry >> 1)
						: tableEntry >> 1;
				}
				return tableEntry;
			}).ToArray();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Calculates the checksum of the byte stream.
		/// </summary>
		/// <param name="byteStream">The byte stream to calculate the checksum for.</param>
		/// <returns>A 32-bit reversed checksum.</returns>
		public static uint Get<T>(IEnumerable<T> byteStream)
		{
			try
			{
				// Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
				return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
						  MChecksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8));
			}
			catch (FormatException e)
			{
				throw new Exception(@"Could not read the stream out as bytes.", e);
			}
			catch (InvalidCastException e)
			{
				throw new Exception(@"Could not read the stream out as bytes.", e);
			}
			catch (OverflowException e)
			{
				throw new Exception(@"Could not read the stream out as bytes.", e);
			}
		}
		#endregion

		#region TobilibiliUID
		private static uint Crc32lastindex(string str)
		{
			var crcstart = 0xFFFFFFFF;
			var len = str.Length;
			uint index = 0;
			for (var i = 0; i < len; ++i)
			{
				index = (crcstart ^ str[i]) & 0xFF;
				crcstart = (crcstart >> 8) ^ MChecksumTable[index];
			}
			return index;
		}

		private static int Getcrcindex(int t)
		{
			for (var i = 0; i < 256; ++i)
			{
				if (MChecksumTable[i] >> 24 == t)
				{
					return i;
				}
			}
			return -1;
		}

		private static string DeepCheck(string i, IReadOnlyList<int> index)
		{
			var str = string.Empty;
			var hash = ~Get(i);
			var tc = hash & 0xff ^ index[2];
			if (!(tc <= 57 && tc >= 48))
				return null;
			str += tc - 48;
			hash = MChecksumTable[index[2]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[1];
			if (!(tc <= 57 && tc >= 48))
				return null;
			str += tc - 48;
			hash = MChecksumTable[index[1]] ^ (hash >> 8);
			tc = hash & 0xff ^ index[0];
			if (!(tc <= 57 && tc >= 48))
				return null;
			str += tc - 48;
			//hash = MChecksumTable[index[0]] ^ (hash >> 8);
			return str;
		}

		private static int Finduidlow(string str)
		{
			for (var i = 0; i < 1000; ++i)
			{
				var arrayOfBytes = Encoding.ASCII.GetBytes(i.ToString(@"D"));
				if (Convert.ToUInt32(str, 16) == Get(arrayOfBytes))
				{
					return i;
				}
			}
			return -1;
		}

		private static int Finduidhigh(string str)
		{
			var index = new int[4];
			var ht = (Convert.ToUInt32(str, 16) ^ 0xffffffff);
			var deepCheckData = string.Empty;
			int i;
			for (i = 3; i >= 0; --i)
			{
				index[3 - i] = Getcrcindex((int)ht >> (i * 8));
				var snum = MChecksumTable[index[3 - i]];
				ht ^= snum >> ((3 - i) * 8);
			}
			for (i = 0; i < 100000; ++i)
			{
				var lastindex = Crc32lastindex(i.ToString());
				if (lastindex == index[3])
				{
					deepCheckData = DeepCheck(i.ToString(), index);
					if (deepCheckData != null)
						break;
				}
			}
			if (i == 100000)
				return -1;
			return Convert.ToInt32(i + deepCheckData);
		}

		public static int Getuid(string str)
		{
			var lowuid = Finduidlow(str);
			if (lowuid != -1)
			{
				return lowuid;
			}
			return Finduidhigh(str);
		}
		#endregion

		#region Fields
		/// <summary>
		/// Contains a cache of calculated checksum chunks.
		/// </summary>
		private static readonly uint[] MChecksumTable;

		#endregion
	}
}
