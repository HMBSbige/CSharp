using BasicLib.Encryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace UnitTest
{
	[TestClass]
	public class Encryption
	{
		[TestMethod]
		public void CBCTest()
		{
			var iv = new byte[16];
			RandomNumberGenerator.Fill(iv);
			var input = new byte[65536];
			RandomNumberGenerator.Fill(input);

			//128
			var key = new byte[16];
			RandomNumberGenerator.Fill(key);
			var encode = StreamEncryptorUtil.CBC_Encrypt(input, key, iv);
			var decode = StreamEncryptorUtil.CBC_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
			//192
			key = new byte[24];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.CBC_Encrypt(input, key, iv);
			decode = StreamEncryptorUtil.CBC_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
			//256
			key = new byte[32];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.CBC_Encrypt(input, key, iv);
			decode = StreamEncryptorUtil.CBC_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
		}

		[TestMethod]
		public void ECBTest()
		{
			var input = new byte[65536];
			RandomNumberGenerator.Fill(input);

			//128
			var key = new byte[16];
			RandomNumberGenerator.Fill(key);
			var encode = StreamEncryptorUtil.ECB_Encrypt(input, key);
			var decode = StreamEncryptorUtil.ECB_Decrypt(encode, key);
			Assert.IsTrue(input.SequenceEqual(decode));
			//192
			key = new byte[24];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.ECB_Encrypt(input, key);
			decode = StreamEncryptorUtil.ECB_Decrypt(encode, key);
			Assert.IsTrue(input.SequenceEqual(decode));
			//256
			key = new byte[32];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.ECB_Encrypt(input, key);
			decode = StreamEncryptorUtil.ECB_Decrypt(encode, key);
			Assert.IsTrue(input.SequenceEqual(decode));
		}

		[TestMethod]
		public void CTRTest()
		{
			var iv = new byte[16];
			RandomNumberGenerator.Fill(iv);
			var input = new byte[65536];
			RandomNumberGenerator.Fill(input);

			//128
			var key = new byte[16];
			RandomNumberGenerator.Fill(key);
			var encode = StreamEncryptorUtil.CTR_Encrypt(input, key, iv);
			var decode = StreamEncryptorUtil.CTR_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
			//192
			key = new byte[24];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.CTR_Encrypt(input, key, iv);
			decode = StreamEncryptorUtil.CTR_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
			//256
			key = new byte[32];
			RandomNumberGenerator.Fill(key);
			encode = StreamEncryptorUtil.CTR_Encrypt(input, key, iv);
			decode = StreamEncryptorUtil.CTR_Decrypt(encode, key, iv);
			Assert.IsTrue(input.SequenceEqual(decode));
		}
	}
}
