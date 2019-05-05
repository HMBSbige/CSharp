using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSharp
{
	public class GithubRelease
	{
		private string _owner;
		private string _repo;

		private const string UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";

		public GithubRelease(string owner, string repo)
		{
			_owner = owner;
			_repo = repo;
		}

		public async Task<(string, string)?> GetLatestAsync()
		{
			var url = $@"https://api.github.com/repos/{_owner}/{_repo}/releases/latest";
			var json = await Get(url, true);
			dynamic result = JsonConvert.DeserializeObject(json);
			if (result[@"html_url"] is string)
			{
				if (result[@"tag_name"] is string)
				{
					return (result[@"html_url"], result[@"tag_name"]);
				}
			}
			return null;
		}

		public async Task<(string, string, bool)?> GetActualLatestAsync()
		{
			var url = $@"https://api.github.com/repos/{_owner}/{_repo}/releases";
			var json = await Get(url, true);
			dynamic result = JsonConvert.DeserializeObject(json);
			if (result[0][@"html_url"] is string)
			{
				if (result[0][@"tag_name"] is string)
				{
					if (result[0][@"prerelease"] is bool)
					{
						return (result[0][@"html_url"], result[0][@"tag_name"], result[0][@"prerelease"]);
					}
				}
			}

			return null;
		}

		private static async Task<string> Get(string uri, bool useProxy = false)
		{
			var httpClientHandler = new HttpClientHandler
			{
				UseProxy = useProxy
			};

			var httpClient = new HttpClient(httpClientHandler);
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add(@"User-Agent", UserAgent);

			var response = await httpClient.SendAsync(request);

			response.EnsureSuccessStatusCode();
			var resultStr = await response.Content.ReadAsStringAsync();

			Debug.WriteLine(resultStr);
			return resultStr;
		}
	}
}
