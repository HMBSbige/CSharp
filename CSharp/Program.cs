using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSharp
{
	class Program
	{
		private const string ipla = @"https://api.ip.la";

		public static async Task<string> Get(string uri, bool useProxy = false)
		{
			var httpClientHandler = new HttpClientHandler
			{
				UseProxy = useProxy
			};
			var httpClient = new HttpClient(httpClientHandler);
			var response = await httpClient.GetAsync(uri);
			response.EnsureSuccessStatusCode();
			var resultStr = await response.Content.ReadAsStringAsync();

			Debug.WriteLine(resultStr);
			return resultStr;
		}

		public static async void Post()
		{
			using var client = new HttpClient();
			client.BaseAddress = new Uri("http://localhost:6740");
			var content = new FormUrlEncodedContent(new[]
			{
					new KeyValuePair<string, string>("", "login")
			});
			var result = await client.PostAsync("/api/Membership/exists", content);
			var resultContent = await result.Content.ReadAsStringAsync();
			Console.WriteLine(resultContent);
		}

		public static void Run()
		{
			// key should be: pre-known, derived, or transported via another channel, such as RSA encryption
			var key = new byte[16];
			RandomNumberGenerator.Fill(key);

			var nonce = new byte[12];
			RandomNumberGenerator.Fill(nonce);

			// normally this would be your data
			var dataToEncrypt = new byte[1234];
			var associatedData = new byte[333];
			RandomNumberGenerator.Fill(dataToEncrypt);
			RandomNumberGenerator.Fill(associatedData);

			// these will be filled during the encryption
			var tag = new byte[16];
			var ciphertext = new byte[dataToEncrypt.Length];

			using (var aesGcm = new AesGcm(key))
			{
				aesGcm.Encrypt(nonce, dataToEncrypt, ciphertext, tag, associatedData);
			}

			// tag, nonce, ciphertext, associatedData should be sent to the other part

			var decryptedData = new byte[ciphertext.Length];

			using (var aesGcm = new AesGcm(key))
			{
				aesGcm.Decrypt(nonce, ciphertext, tag, decryptedData, associatedData);
			}

			// do something with the data
			// this should always print that data is the same
			Console.WriteLine(
					$"AES-GCM: Decrypted data is {(dataToEncrypt.SequenceEqual(decryptedData) ? "the same as" : "different than")} original data.");
		}

		public static byte[] EncryptStringToBytes_Aes(byte[] plainText, byte[] Key, byte[] IV, CipherMode mode)
		{
			// Check arguments.
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Create an AesManaged object
			// with the specified key and IV.
			using var aesAlg = new AesManaged
			{
				Key = Key,
				IV = IV,
				Mode = mode
			};

			// Create an encryptor to perform the stream transform.
			var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using var msEncrypt = new MemoryStream();
			using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
			csEncrypt.Write(plainText, 0, plainText.Length);
			csEncrypt.FlushFinalBlock();
			return msEncrypt.ToArray();
		}

		public static byte[] DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV, CipherMode mode)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Declare the string used to hold
			// the decrypted text.

			// Create an AesManaged object
			// with the specified key and IV.
			using var aesAlg = new AesManaged
			{
				Key = Key,
				IV = IV,
				Mode = mode
			};

			// Create a decryptor to perform the stream transform.
			var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for decryption.
			using var msDecrypt = new MemoryStream();
			using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);
			csDecrypt.Write(cipherText, 0, cipherText.Length);
			csDecrypt.FlushFinalBlock();
			return msDecrypt.ToArray();
		}

		private static void Main(string[] args)
		{
			//DNSPoison.Test(@"D:\Downloads\2.txt");
			var original = "Here is some data to encrypt!";

			// Create a new instance of the AesManaged
			// class.  This generates a new key and initialization 
			// vector (IV).
			using (var myAes = new AesManaged())
			{
				myAes.Mode = CipherMode.CBC;
				//The CFB and OFB modes are not supported.

				// Encrypt the string to an array of bytes.
				var encrypted = EncryptStringToBytes_Aes(Encoding.UTF8.GetBytes(original), myAes.Key, myAes.IV, myAes.Mode);

				// Decrypt the bytes to a string.
				var roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV, myAes.Mode);

				//Display the original data and the decrypted data.
				Console.WriteLine("Original:   {0}", original);
				Console.WriteLine("Round Trip: {0}", Encoding.UTF8.GetString(roundtrip));
			}

			Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
