using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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

		private static void Main(string[] args)
		{
			DNSPoison.Test(@"D:\Downloads\2.txt");


			Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
