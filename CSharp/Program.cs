using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:6740");
				var content = new FormUrlEncodedContent(new[]
				{
						new KeyValuePair<string, string>("", "login")
				});
				var result = await client.PostAsync("/api/Membership/exists", content);
				string resultContent = await result.Content.ReadAsStringAsync();
				Console.WriteLine(resultContent);
			}
		}

		public static void Run()
		{
			// key should be: pre-known, derived, or transported via another channel, such as RSA encryption
			byte[] key = new byte[16];
			RandomNumberGenerator.Fill(key);

			byte[] nonce = new byte[12];
			RandomNumberGenerator.Fill(nonce);

			// normally this would be your data
			byte[] dataToEncrypt = new byte[1234];
			byte[] associatedData = new byte[333];
			RandomNumberGenerator.Fill(dataToEncrypt);
			RandomNumberGenerator.Fill(associatedData);

			// these will be filled during the encryption
			byte[] tag = new byte[16];
			byte[] ciphertext = new byte[dataToEncrypt.Length];

			using (AesGcm aesGcm = new AesGcm(key))
			{
				aesGcm.Encrypt(nonce, dataToEncrypt, ciphertext, tag, associatedData);
			}

			// tag, nonce, ciphertext, associatedData should be sent to the other part

			byte[] decryptedData = new byte[ciphertext.Length];

			using (AesGcm aesGcm = new AesGcm(key))
			{
				aesGcm.Decrypt(nonce, ciphertext, tag, decryptedData, associatedData);
			}

			// do something with the data
			// this should always print that data is the same
			Console.WriteLine(
					$"AES-GCM: Decrypted data is {(dataToEncrypt.SequenceEqual(decryptedData) ? "the same as" : "different than")} original data.");
		}

		private static void Main(string[] args)
		{
			//DNSPoison.Test(@"D:\Downloads\2.txt");

			Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
