using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkLib.Utils
{
	/// <summary>
	/// Performs 32-bit reversed cyclic redundancy checks.
	/// </summary>
	public static class CRC32
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
		static CRC32()
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
		public static uint Get(string str, Encoding encoding = null)
		{
			try
			{
				if (encoding == null)
				{
					encoding = new UTF8Encoding(false);
				}
				var byteStream = encoding.GetBytes(str);
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

		public static uint Get<T>(IEnumerable<T> byteStream)
		{
			try
			{
				// Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
				return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
						(MChecksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
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

		#region Fields
		/// <summary>
		/// Contains a cache of calculated checksum chunks.
		/// </summary>
		public static readonly uint[] MChecksumTable;

		#endregion
	}
}
