using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetUtils
{
	public static class HttpContentExtensions
	{
		public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
		{
			var pathname = Path.GetFullPath(filename);
			if (!overwrite && File.Exists(filename))
			{
				throw new InvalidOperationException($@"File {pathname} already exists.");
			}

			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
				return content.CopyToAsync(fileStream).ContinueWith(copyTask => { fileStream.Close(); });
			}
			catch
			{
				fileStream?.Close();
				throw;
			}
		}

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
	}
}
