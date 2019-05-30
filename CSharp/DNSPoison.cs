using DnsClient;
using DnsClient.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CSharp
{
	public static class DNSPoison
	{
		private static async Task<string> Get(string uri, bool useProxy = false)
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

		private static uint IPv42UintBE(IPAddress ipv4)
		{
			var buf = ipv4.GetAddressBytes();
			Array.Reverse(buf);
			return BitConverter.ToUInt32(buf, 0);
		}

		private static string ToAddress(long address)
		{
			return IPAddress.Parse(address.ToString()).ToString();
		}

		private static IPAddress GetAnswer()
		{
			try
			{
				var endpoint = new IPEndPoint(IPAddress.Parse(@"8.8.8.8"), 53);
				var client = new LookupClient(endpoint)
				{
					UseCache = false
				};
				return client.Query(@"www.google.com", QueryType.A).Answers.OfType<ARecord>().FirstOrDefault()?.Address;

			}
			catch
			{
				return null;
			}
		}

		private static bool IsGoogle(IPAddress ip)
		{
			var str = Get($@"http://freeapi.ipip.net/{ip}").Result.ToLower();
			if (str.Contains(@"google.com"))
			{
				//Console.WriteLine($@"{ip}:True");
				return true;
			}
			else
			{
				//Console.WriteLine($@"{ip}:False");
				return false;
			}
		}

		public static void Test(string path)
		{
			var list = new SortedSet<uint>();

			if (File.Exists(path))
			{
				var originStr = File.ReadAllLines(path);
				foreach (var s in originStr)
				{
					if (IPAddress.TryParse(s, out var ip))
					{
						list.Add(IPv42UintBE(ip));
					}
				}
			}

			for (var i = 0; i < 500;)
			{
				var ip = GetAnswer();
				if (ip != null && ip.AddressFamily == AddressFamily.InterNetwork)
				{
					if (!list.Contains(IPv42UintBE(ip)))
					{
						if (!IsGoogle(ip))
						{
							list.Add(IPv42UintBE(ip));
							++i;
							Save(path, list);
							Console.WriteLine($@"{i}	{ip}");
						}

						Task.Delay(200).Wait();
					}
				}
			}
		}

		private static void Save(string path, IEnumerable<uint> list)
		{
			var strList = list.Select(ipAddress => ToAddress(ipAddress));

			File.WriteAllLines(path, strList);
		}

	}
}
