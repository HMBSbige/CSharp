using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BasicLib.Encryption
{
	public static class StreamEncryptorUtil
	{
		#region private method

		//The CFB, CTS and OFB modes are not supported.
		private static byte[] AesEncrypt(byte[] input, byte[] key, byte[] iv, CipherMode mode)
		{
			if (input == null || input.Length <= 0)
				throw new ArgumentNullException("input");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("key");
			if (iv != null && iv.Length <= 0)
				throw new ArgumentNullException("iv");

			using var aesAlg = new AesManaged
			{
				Key = key,
				Mode = mode
			};
			if (iv != null)
			{
				aesAlg.IV = iv;
			}

			var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			using var msEncrypt = new MemoryStream();
			using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
			csEncrypt.Write(input, 0, input.Length);
			csEncrypt.FlushFinalBlock();
			return msEncrypt.ToArray();
		}

		//The CFB, CTS and OFB modes are not supported.
		private static byte[] AesDecrypt(byte[] input, byte[] key, byte[] iv, CipherMode mode)
		{
			if (input == null || input.Length <= 0)
				throw new ArgumentNullException("input");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("key");
			if (iv != null && iv.Length <= 0)
				throw new ArgumentNullException("iv");

			using var aesAlg = new AesManaged
			{
				Key = key,
				Mode = mode
			};
			if (iv != null)
			{
				aesAlg.IV = iv;
			}

			var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			using var msDecrypt = new MemoryStream();
			using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);
			csDecrypt.Write(input, 0, input.Length);
			csDecrypt.FlushFinalBlock();
			return msDecrypt.ToArray();
		}

		private static void AesCtrTransform(byte[] key, byte[] salt, Stream inputStream, Stream outputStream)
		{
			using SymmetricAlgorithm aes = new AesManaged { Mode = CipherMode.ECB, Padding = PaddingMode.None };

			var blockSize = aes.BlockSize / 8;

			if (salt.Length != blockSize)
			{
				throw new ArgumentException($@"Salt size must be same as block size (actual: {salt.Length}, expected: {blockSize})");
			}

			var counter = (byte[])salt.Clone();

			var xorMask = new Queue<byte>();

			var zeroIv = new byte[blockSize];
			using var counterEncryptor = aes.CreateEncryptor(key, zeroIv);

			int b;
			while ((b = inputStream.ReadByte()) != -1)
			{
				if (xorMask.Count == 0)
				{
					var counterModeBlock = new byte[blockSize];

					counterEncryptor.TransformBlock(counter, 0, counter.Length, counterModeBlock, 0);

					for (var i2 = 0; i2 < counter.Length; ++i2)
					{
						if (++counter[i2] != 0)
						{
							break;
						}
					}

					foreach (var b2 in counterModeBlock)
					{
						xorMask.Enqueue(b2);
					}
				}

				var mask = xorMask.Dequeue();
				outputStream.WriteByte((byte)((byte)b ^ mask));
			}
		}

		#endregion

		#region public method

		public static byte[] CBC_Encrypt(byte[] input, byte[] key, byte[] iv)
		{
			return AesEncrypt(input, key, iv, CipherMode.CBC);
		}

		public static byte[] CBC_Decrypt(byte[] input, byte[] key, byte[] iv)
		{
			return AesDecrypt(input, key, iv, CipherMode.CBC);
		}

		public static byte[] ECB_Encrypt(byte[] input, byte[] key)
		{
			return AesEncrypt(input, key, null, CipherMode.ECB);
		}

		public static byte[] ECB_Decrypt(byte[] input, byte[] key)
		{
			return AesDecrypt(input, key, null, CipherMode.ECB);
		}

		public static byte[] CTR_Encrypt(byte[] input, byte[] key, byte[] iv)
		{
			using var inputStream = new MemoryStream(input);
			using var outputStream = new MemoryStream();
			AesCtrTransform(key, iv, inputStream, outputStream);
			return outputStream.ToArray();
		}

		public static byte[] CTR_Decrypt(byte[] input, byte[] key, byte[] iv)
		{
			return CTR_Encrypt(input, key, iv);
		}

		#endregion
	}
}
