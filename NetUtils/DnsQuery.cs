using OpenDNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace NetUtils
{
	internal class DnsQuery
	{
		private delegate IPHostEntry GetHostEntryHandler(string ip);

		public int Timeout = 2000;
		private readonly OpenDNS.DnsQuery _dns = new OpenDNS.DnsQuery();

		public DnsQuery(IEnumerable<IPEndPoint> ips)
		{
			foreach (var server in ips)
			{
				_dns.Servers.Add(server);
			}

			_dns.Timeout = Timeout;
			_dns.RecursionDesired = true;
		}

		public DnsQuery() : this(IPFormatter.ToIPEndPoints(GetLocalDnsAddress(), 53)) { }

		private static bool IsWorkedInterface(NetworkInterface networkInterface)
		{
			if (networkInterface.OperationalStatus == OperationalStatus.Up
			&& (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
				networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
			{
				return true;
			}

			return false;
		}

		public static IEnumerable<IPAddress> GetLocalDnsAddress()
		{
			var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			return from networkInterface in networkInterfaces
				   where IsWorkedInterface(networkInterface)
				   select networkInterface.GetIPProperties()
				   into ipProperties
				   from dns in ipProperties.DnsAddresses
				   where IPFormatter.IsIPv4Address(dns)
				   select dns;
		}

		public static IEnumerable<IPAddress> GetRouteDnsAddress()
		{
			var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			return from networkInterface in networkInterfaces
				   where IsWorkedInterface(networkInterface)
				   select networkInterface.GetIPProperties()
				   into ipProperties
				   from dns in ipProperties.GatewayAddresses
				   where IPFormatter.IsIPv4Address(dns.Address)
				   select dns.Address;
		}

		[Obsolete]
		public static string GetHostName(IPAddress ip, int timeout)
		{
			try
			{
				var callback = new GetHostEntryHandler(Dns.GetHostEntry);
				var result = callback.BeginInvoke(ip.ToString(), null, null);
				if (result.AsyncWaitHandle.WaitOne(timeout, false))
				{
					return callback.EndInvoke(result).HostName;
				}
				else
				{
					return ip.ToString();
				}
			}
			catch (Exception)
			{
				return ip.ToString();
			}
		}

		private string QueryString(string queryStr, Types type)
		{
			_dns.Domain = queryStr;
			_dns.QueryType = type;
			if (_dns.Send())
			{
				var count = _dns.Response.Answers.Count;
				if (count > 0)
				{
					for (var i = 0; i < count; ++i)
					{
						if (((ResourceRecord)_dns.Response.Answers[i]).Type != type)
						{
							continue;
						}

						return ((ResourceRecord)_dns.Response.Answers[i]).RText;
					}
				}
			}

			return string.Empty;
		}

		private IPAddress QueryAddress(string hostname, Types type)
		{
			_dns.Domain = hostname;
			_dns.QueryType = type;
			if (_dns.Send())
			{
				var count = _dns.Response.Answers.Count;
				if (count > 0)
				{
					for (var i = 0; i < count; ++i)
					{
						if (((ResourceRecord)_dns.Response.Answers[i]).Type != type)
						{
							continue;
						}

						return ((Address)_dns.Response.Answers[i]).IP;
					}
				}
			}

			return null;
		}

		public string Ptr(IPAddress ip)
		{
			return Ptr(ip.ToString());
		}

		public string Ptr(string queryStr)
		{
			return QueryString(IPFormatter.IPStr2PTRName(queryStr), Types.PTR);
		}

		public string CName(string hostname)
		{
			return QueryString(hostname, Types.CNAME);
		}

		public IPAddress A(string hostname)
		{
			return QueryAddress(hostname, Types.A);
		}

		public IPAddress AAAA(string hostname)
		{
			return QueryAddress(hostname, Types.AAAA);
		}
	}
}
