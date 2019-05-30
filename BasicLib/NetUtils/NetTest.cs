using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BasicLib.NetUtils
{
	internal static class NetTest
	{
		public static async Task<IPAddress> GetPublicIpAddress()
		{
			var httpClient = new HttpClient();
			var ip = await httpClient.GetStringAsync(@"http://api.ip.la");
			Debug.WriteLine($@"Public IP address is: {ip}");
			return IPAddress.Parse(ip.Trim());
		}
	}
}