﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	internal static class NetTest
	{
		public class PingStatus
		{
			public PingStatus()
			{
				Status = IPStatus.Unknown;
				Address = null;
				RTT = 0;
				TTL = 0;
				bytes = 0;
			}
			public IPStatus Status;
			public IPAddress Address;
			public long RTT;
			public int TTL;
			public int bytes;
		}

		public static async Task<IPAddress> GetIP(string host)
		{
			try
			{
				var ips = await Dns.GetHostAddressesAsync(host);
				var res = ips[0];
				Debug.WriteLine($@"DNS query {host} answer {res}");
				return res;
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null)
				{
					ex = ex.InnerException;
				}
				Debug.WriteLine($@"ERROR:{ex.Message}");
				return null;
			}
		}

		public static async Task<string> GetHostName(IPAddress ip)
		{
			string hostname;
			try
			{
				var hostsEntry = await Dns.GetHostEntryAsync(ip);
				hostname = hostsEntry.HostName;
			}
			catch
			{
				hostname = ip.ToString();
			}
			return hostname;
		}

		public static async Task<double?> TCPing(IPAddress ip, int port = 80, int timeout = 1000)
		{
			if (ip == null)
			{
				return null;
			}
			using (var client = new TcpClient())
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				await Task.WhenAny(client.ConnectAsync(ip, port), Task.Delay(timeout));
				stopwatch.Stop();
				var t = stopwatch.Elapsed.TotalMilliseconds;
				if (client.Connected == false)
				{
					Debug.WriteLine($@"TCPing {ip}:{port}:超时({t}ms > {timeout}ms)");
					return null;
				}
				Debug.WriteLine($@"TCPing {ip}:{port}:{t:0.00}ms");
				return t;
			}
		}

		public static async Task<PingStatus> ICMPing(IPAddress ip, int timeout = 1000)
		{
			var res = new PingStatus();
			if (ip == null)
			{
				return res;
			}

			var p1 = new Ping();
			var reply = await p1.SendPingAsync(ip, timeout);
			if (reply != null && reply.Status == IPStatus.Success)
			{
				res.Status = reply.Status;
				res.Address = reply.Address;
				res.RTT = reply.RoundtripTime;
				res.TTL = reply.Options.Ttl;
				res.bytes = reply.Buffer.Length;
				//Debug info
				var sb = new StringBuilder();
				sb.AppendLine($@"Status: {res.Status}");
				sb.AppendLine($@"Address: {res.Address}");
				sb.AppendLine($@"RTT: {res.RTT}");
				sb.AppendLine($@"TTL: {res.TTL}");
				sb.AppendLine($@"Buffer size: {res.bytes}");
				Debug.WriteLine(sb.ToString());
			}
			else if (reply != null && reply.Status == IPStatus.TimedOut)
			{
				Debug.WriteLine(@"超时");
				res.Status = reply.Status;
			}
			else
			{
				Debug.WriteLine(@"失败");
				res.Status = IPStatus.Unknown;
			}

			return res;
		}

		public static async Task<double?> IsPortOpen(IPAddress ip, int port = 80, uint warmup = 1, uint n = 4, int timeout = 1000)
		{
			var times = new List<double>();
			for (uint i = 0; i < warmup; ++i)
			{
				var result = await TCPing(ip, port, timeout);
				if (result != null)
				{
					Debug.WriteLine($@"[Warmup]Connected to {ip}:{port}:{result}ms");
				}
			}
			for (uint i = 0; i < n; ++i)
			{
				var result = await TCPing(ip, port, timeout);
				if (result != null)
				{
					Debug.WriteLine($@"Connected to {ip}:{port}:{result}ms");
					times.Add(result.Value);
				}
			}

			if (times.Count == 0)
			{
				return null;
			}
			else
			{
				Debug.WriteLine($@"Average :{times.Average()}ms");
				return Math.Round(times.Average(), 2);
			}
		}

		public static async Task<IPAddress> GetPublicIpAddress()
		{
			var httpClient = new HttpClient();
			var ip = await httpClient.GetStringAsync(@"http://api.ip.la");
			Debug.WriteLine($@"Public IP address is: {ip}");
			return IPAddress.Parse(ip.Trim());
		}
	}
}
