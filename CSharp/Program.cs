using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharp
{
	class Program
	{
		private const string ipla = @"https://api.ip.la";

		public static async Task<string> Get(string uri)
		{
			var httpClient = new HttpClient();
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

		private static IEnumerable<string> GetParameter(string s)
		{
			var pattern = Regex.Escape(@"/*const char **/") + @" (\w+)";
			var parameters = Regex.Matches(s, pattern);
			var list = new List<string>();
			foreach (Match parameter in parameters)
			{
				list.Add(parameter.Groups[1].Value);
			}
			return list;
		}

		private static string GetFunction(string s)
		{
			var pattern = @"(\w+)\(.*\)";
			return Regex.Match(s, pattern).Groups[1].Value;
		}

		private static IEnumerable<string> GetMdLine(string origin)
		{
			if (origin.Contains(@"const char *"))
			{
				var func = GetFunction(origin).Split('_');
				var f1 = func[0];
				var f2 = func[1];

				var sb = $@"{GetFunction(origin).Replace(@"_", @"::")}|Parameter|";

				var parameters = GetParameter(origin);

				return parameters.Select(parameter => $@"{f1}::{f2}|Parameter|{parameter}|[Undefined](https://partner.steamgames.com/doc/api/{f1}#{f2})|ASCII|<li>- [ ] </li>").ToList();
			}
			return null;
		}

		private static void Main(string[] args)
		{

			var lines = File.ReadAllLines(@"D:\Downloads\SteamNative.Platform.Interface.cs");
			var res = new StringBuilder();
			foreach (var line in lines)
			{
				var newLine = GetMdLine(line);
				if (newLine != null)
				{
					foreach (var l in newLine)
					{
						res.AppendLine(l);
					}
				}
			}
			File.WriteAllText(@"D:\Downloads\SteamString.cs", res.ToString());
			
			Console.WriteLine(Environment.NewLine + @"END OF CONSOLE" + Environment.NewLine);
			Console.ReadKey();
		}
	}
}
