using BasicLib.Utils;
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
			var dataToEncrypt = new byte[32];
			var associatedData = new byte[33];
			RandomNumberGenerator.Fill(dataToEncrypt);
			RandomNumberGenerator.Fill(associatedData);

			var (ciphertext, tag) = GCM_Encrypt(key, nonce, dataToEncrypt, associatedData);

			var decryptedData = GCM_Decrypt(key, nonce, ciphertext, associatedData, tag);

			Utils.OutputBytes(key);
			Utils.OutputBytes(nonce);

			Utils.OutputBytes(dataToEncrypt);
			Utils.OutputBytes(associatedData);

			Utils.OutputBytes(ciphertext);
			Utils.OutputBytes(tag);

			Utils.OutputBytes(decryptedData);

			// do something with the data
			// this should always print that data is the same
			Console.WriteLine($"AES-GCM: Decrypted data is {(dataToEncrypt.SequenceEqual(decryptedData) ? "the same as" : "different than")} original data.");
		}

		public static (byte[], byte[]) GCM_Encrypt(byte[] key, byte[] nonce, byte[] input, byte[] associatedData)
		{
			var tag = new byte[16];
			var output = new byte[input.Length];

			using var aesGcm = new AesGcm(key);
			aesGcm.Encrypt(nonce, input, output, tag, associatedData);

			return (output, tag);
		}

		public static byte[] GCM_Decrypt(byte[] key, byte[] nonce, byte[] input, byte[] associatedData, byte[] tag)
		{
			var output = new byte[input.Length];

			using var aesGcm = new AesGcm(key);
			aesGcm.Decrypt(nonce, input, tag, output, associatedData);

			return output;
		}

		private static void Main(string[] args)
		{
			//DNSPoison.Test(@"D:\Downloads\2.txt");
			Run();
			Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
