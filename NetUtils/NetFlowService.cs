using Common;
using CommonControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetUtils
{
	public class NetFlowService
	{
		public bool IsNetFlowRun { get; private set; } = false;

		public bool IsNetPacketRun { get; private set; } = false;

		/// <summary>
		/// 程序联网信息列表
		/// </summary>
		public List<NetProcessInfo> NetProcessInfoList { get; } = new List<NetProcessInfo>();

		/// <summary>
		/// 网络流量计数器
		/// </summary>
		public NetFlowTool NetFlow { get; } = new NetFlowTool();

		private readonly List<NetPacketTool> NetPacketList = new List<NetPacketTool>();

		NetProcessTool.TcpRow[] TcpConnection;
		NetProcessTool.UdpRow[] UdpConnection;
		Process[] NowProcess;
		List<string> AllIPv4Address = new List<string>();

		public long LostPacketCount { get; set; }

		/// <summary>
		/// 开启网络流量监控
		/// </summary>
		public void Start()
		{
			#region 启动系统性能计数器统计
			try
			{
				NetFlow.Start();
				NetFlow.DataMonitorEvent += DataMonitorEvent;
				IsNetFlowRun = true;
			}
			catch
			{
				// ignored
			}

			#endregion

			#region 启动Socket包统计
			if (CheckPermission.IsAdmin())
			{
				var hosts = NetCardInfoTool.GetIPv4Address();
				AllIPv4Address = NetCardInfoTool.GetAllIPv4Address();
				foreach (var host in hosts)
				{
					try
					{
						var p = new NetPacketTool(host);
						p.NewPacket += NewPacketEvent;
						p.Start();
						NetPacketList.Add(p);
					}
					catch
					{
						// ignored
					}
				}
				if (ListTool.HasElements(NetPacketList)) IsNetPacketRun = true;
			}
			#endregion
		}
		/// <summary>
		/// 关闭网络流量监控
		/// </summary>
		public void Stop()
		{
			if (IsNetFlowRun)
			{
				NetFlow.Stop();
				IsNetFlowRun = false;
			}

			if (IsNetPacketRun)
			{
				NetPacketList.ForEach(x => { x.Stop(); });
				IsNetPacketRun = false;
			}
		}

		/// <summary>
		/// 系统性能计数器每秒统计事件
		/// </summary>
		/// <param name="n"></param>
		public void DataMonitorEvent(NetFlowTool n)
		{
			NowProcess = Process.GetProcesses();
			GetConnection();
			SetNetProcess();
			CalcNetProcessInfo();

			CheckRestart();
		}
		/// <summary>
		/// 整理数据包到所属的进程
		/// </summary>
		/// <param name="tool"></param>
		/// <param name="packet"></param>
		private void NewPacketEvent(NetPacketTool tool, Packet packet)
		{
			var isGather = false;
			#region 整理TCP包
			if (packet.Protocol == Protocol.Tcp && ListTool.HasElements(TcpConnection) && ListTool.HasElements(NowProcess))
			{
				lock (TcpConnection)
				{
					// tcp 下载
					if (TcpConnection.Any(x => x.RemoteIP.ToString() == packet.DestinationAddress.ToString() && x.RemotePort == packet.DestinationPort))
					{
						var tcpDownload = TcpConnection.FirstOrDefault(x => x.RemoteIP.ToString() == packet.DestinationAddress.ToString() && x.RemotePort == packet.DestinationPort);
						var process = NowProcess.FirstOrDefault(x => x.Id == tcpDownload.ProcessId);
						if (process != null)
						{
							var info = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == process.ProcessName);
							if (info != null)
							{
								isGather = true;
								info.DownloadBag += packet.TotalLength;
								info.DownloadBagCount += packet.TotalLength;
							}
						}
					}
					// tcp 上传
					if (TcpConnection.Any(x => x.LocalIP.ToString() == packet.SourceAddress.ToString() && x.LocalPort == packet.SourcePort))
					{
						var tcUpload = TcpConnection.FirstOrDefault(x => x.LocalIP.ToString() == packet.SourceAddress.ToString() && x.LocalPort == packet.SourcePort);
						var process = NowProcess.FirstOrDefault(x => x.Id == tcUpload.ProcessId);
						if (process != null)
						{
							var info = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == process.ProcessName);
							if (info != null)
							{
								isGather = true;
								info.UploadBag += packet.TotalLength;
								info.UploadBagCount += packet.TotalLength;
							}
						}
					}
				}
			}
			#endregion
			#region 整理UDP包
			if (packet.Protocol == Protocol.Udp && ListTool.HasElements(UdpConnection) && ListTool.HasElements(NowProcess))
			{
				lock (UdpConnection)
				{
					// tcp 下载
					if (UdpConnection.Any(x => x.LocalPort == packet.DestinationPort) && AllIPv4Address.Contains(packet.DestinationAddress.ToString()))
					{
						var udpDownload = UdpConnection.FirstOrDefault(x => AllIPv4Address.Contains(x.LocalIP.ToString()) && x.LocalPort == packet.DestinationPort);
						var process = NowProcess.FirstOrDefault(x => x.Id == udpDownload.ProcessId);
						if (process != null)
						{
							var info = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == process.ProcessName);
							if (info != null)
							{
								isGather = true;
								info.DownloadBag += packet.TotalLength;
								info.DownloadBagCount += packet.TotalLength;
								if (info.ProcessName == @"Idle")
								{

								}
							}
						}
					}
					// udp 上传
					if (UdpConnection.Any(x => x.LocalPort == packet.SourcePort) && AllIPv4Address.Contains(packet.SourceAddress.ToString()))
					{
						//var udpIp = AllIPv4Address.FirstOrDefault(x => x == packet.SourceAddress.ToString());
						var ucUpload = UdpConnection.FirstOrDefault(x => AllIPv4Address.Contains(x.LocalIP.ToString()) && x.LocalPort == packet.SourcePort);
						var process = NowProcess.FirstOrDefault(x => x.Id == ucUpload.ProcessId);
						if (process != null)
						{
							var info = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == process.ProcessName);
							if (info != null)
							{
								isGather = true;
								info.UploadBag += packet.TotalLength;
								info.UploadBagCount += packet.TotalLength;
								if (info.ProcessName == @"Idle")
								{

								}
							}
						}
					}
				}
			}
			#endregion
			if (!isGather)
			{
				LostPacketCount++;
			}
		}

		#region 获取所有网络连接
		/// <summary>
		/// 获取所有网络连接并整理列表
		/// </summary>
		void GetConnection()
		{
			TcpConnection = NetProcessTool.GetTcpConnection();
			UdpConnection = NetProcessTool.GetUdpConnection();
		}
		#endregion
		#region 设置程序流量及连接数统计列表
		/// <summary>
		/// 清空并重置当前所有程序的连接数
		/// </summary>
		void SetNetProcess()
		{
			// 清空已有连接数
			if (ListTool.HasElements(NetProcessInfoList))
				NetProcessInfoList.ForEach(x =>
				{
					x.NetConnectionInfoList = new List<NetConnectionInfo>();
				});

			// 统计TCP连接数
			if (ListTool.HasElements(TcpConnection))
			{
				foreach (var t in TcpConnection)
				{
					SetNetProcessConnection(t);
				}
			}
			// 统计UDP连接数
			if (ListTool.HasElements(UdpConnection))
			{
				foreach (var u in UdpConnection)
				{
					SetNetProcessConnection(u);
				}
			}
		}
		/// <summary>
		/// 整理TCP连接到所属的进程
		/// </summary>
		/// <param name="t"></param>
		void SetNetProcessConnection(NetProcessTool.TcpRow t)
		{
			try
			{
				var p = NowProcess.FirstOrDefault(x => x.Id == t.ProcessId);
				if (p != null)
				{
					var ppl = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == p.ProcessName);
					if (ppl == null)
					{
						NetProcessInfoList.Add(
							new NetProcessInfo()
							{
								ProcessId = p.Id,
								ProcessIcon = ProcessInfoTool.GetIcon(p, false),
								ProcessName = p.ProcessName,
								LastUpdateTime = DateTime.Now,
								NetConnectionInfoList = new List<NetConnectionInfo>() {
									new NetConnectionInfo() {
										LocalIP = t.LocalIP.ToString(),
										LocalPort = t.LocalPort,
										RemoteIP = t.RemoteIP.ToString(),
										RemotePort = t.RemotePort,
										ProtocolName = "TCP",
										Status = t.State,
										LastUpdateTime = DateTime.Now,
									}
								},
							});
					}
					else
					{
						ppl.LastUpdateTime = DateTime.Now;
						var conn = ppl.NetConnectionInfoList.FirstOrDefault(x => x.LocalIP == t.LocalIP.ToString() && x.LocalPort == t.LocalPort && x.RemoteIP == t.RemoteIP.ToString() && x.RemotePort == t.RemotePort);
						if (conn == null)
						{
							ppl.NetConnectionInfoList.Add(new NetConnectionInfo()
							{
								LocalIP = t.LocalIP.ToString(),
								LocalPort = t.LocalPort,
								RemoteIP = t.RemoteIP.ToString(),
								RemotePort = t.RemotePort,
								ProtocolName = "TCP",
								Status = t.State,
								LastUpdateTime = DateTime.Now,
							});
						}
					}
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}
		/// <summary>
		/// 整理UDP连接到所属的进程
		/// </summary>
		/// <param name="u"></param>
		void SetNetProcessConnection(NetProcessTool.UdpRow u)
		{
			try
			{
				var p = NowProcess.FirstOrDefault(x => x.Id == u.ProcessId);
				if (p != null)
				{
					var ppl = NetProcessInfoList.FirstOrDefault(x => x.ProcessName == p.ProcessName);
					if (ppl == null)
					{
						NetProcessInfoList.Add(
							new NetProcessInfo()
							{
								ProcessId = p.Id,
								ProcessIcon = ProcessInfoTool.GetIcon(p, false),
								ProcessName = p.ProcessName,
								LastUpdateTime = DateTime.Now,
								NetConnectionInfoList = new List<NetConnectionInfo>() {
									new NetConnectionInfo() {
										LocalIP = u.LocalIP.ToString(),
										LocalPort = u.LocalPort,
										ProtocolName = "UDP",
										LastUpdateTime = DateTime.Now,
									}
								},
							});
					}
					else
					{
						ppl.LastUpdateTime = DateTime.Now;
						var conn = ppl.NetConnectionInfoList.FirstOrDefault(x => x.LocalIP == u.LocalIP.ToString() && x.LocalPort == u.LocalPort);
						if (conn == null)
						{
							ppl.NetConnectionInfoList.Add(new NetConnectionInfo()
							{
								LocalIP = u.LocalIP.ToString(),
								LocalPort = u.LocalPort,
								ProtocolName = "UDP",
								LastUpdateTime = DateTime.Now,
							});
						}
					}
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}
		#endregion
		#region 整理程序流量汇总信息
		/// <summary>
		/// 整理计算程序网络流量
		/// </summary>
		void CalcNetProcessInfo()
		{
			if (ListTool.HasElements(NetProcessInfoList))
			{
				NetProcessInfoList.ForEach(p =>
				{
					p.UploadDataCount += p.UploadData;
					p.DownloadDataCount += p.DownloadData;
				});

				var allupbag = NetProcessInfoList.Sum(x => x.UploadBag);
				var alldownbag = NetProcessInfoList.Sum(x => x.DownloadBag);

				NetProcessInfoList.ForEach(p =>
				{
					if (allupbag > 0 && NetFlow.UploadData > 0)
					{
						var uprate = (float)p.UploadBag / allupbag;
						p.UploadData = (int)(uprate * NetFlow.UploadData);
					}
					if (alldownbag > 0 && NetFlow.DownloadData > 0)
					{
						var downrate = (float)p.DownloadBag / alldownbag;
						p.DownloadData = (int)(downrate * NetFlow.DownloadData);
					}

					p.UploadBag = 0;
					p.DownloadBag = 0;
					p.LastUpdateTime = DateTime.Now;
				});
			}
		}
		#endregion
		#region 联网断网重启计划
		/// <summary>
		/// 联网断网重启计划（应对断网或重连后网卡抓包报错造成的不准确）
		/// </summary>
		private void CheckRestart()
		{
			var rest = false;

			var ints = NetCardInfoTool.GetInstanceNames();
			if (ListTool.IsNullOrEmpty(NetFlow.Instances) && ListTool.HasElements(ints))
			{
				rest = true;
			}
			if (ListTool.HasElements(NetFlow.Instances) && ListTool.HasElements(ints) &&
				string.Join("-", NetFlow.Instances) != string.Join("-", ints))
			{
				rest = true;
			}

			if (rest)
			{
				//重启 系统性能计数器
				NetFlow.Restart();
				//重启 抓包监听
				var hosts = NetCardInfoTool.GetIPv4Address();
				AllIPv4Address = NetCardInfoTool.GetAllIPv4Address();
				foreach (var host in hosts)
				{
					try
					{
						if (NetPacketList.All(x => x.IP.ToString() != host.ToString()))
						{
							var p = new NetPacketTool(host);
							p.NewPacket += NewPacketEvent;
							p.Start();
							NetPacketList.Add(p);
						}
					}
					catch
					{
						// ignored
					}
				}
			}
		}
		#endregion
	}
}
