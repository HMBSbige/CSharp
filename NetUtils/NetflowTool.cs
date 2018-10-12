using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetUtils
{
	public class NetFlowTool
	{
		/// <summary>
		/// 上行数据流量
		/// </summary>
		public int UploadData { get; private set; }

		/// <summary>
		/// 上行数据总流量
		/// </summary>
		public long UploadDataCount { get; private set; }

		/// <summary>
		/// 下行数据流量
		/// </summary>
		public int DownloadData { get; private set; }

		/// <summary>
		/// 下行数据总流量
		/// </summary>
		public long DownloadDataCount { get; private set; }

		private List<PerformanceCounter> UploadCounter, DownloadCounter;//上行、下行流量计数器
		private int DataCounterInterval = 1000;//数据流量计数器计数周期
		private bool IsStart = false;

		public delegate void MonitorEvent(NetFlowTool n);
		public MonitorEvent DataMonitorEvent;

		public string[] Instances { get; private set; }

		private bool Init()
		{
			Instances = NetCardInfoTool.GetInstanceNames();
			if (ListTool.HasElements(Instances))
			{
				UploadCounter = new List<PerformanceCounter>();
				DownloadCounter = new List<PerformanceCounter>();
				for (int i = 0; i < Instances.Length; ++i)
				{
					try
					{
						// 添加 上行流量计数器
						UploadCounter.Add(new PerformanceCounter(@"Network Interface", @"Bytes Sent/sec", Instances[i]));
					}
					catch
					{
						// ignored
					}

					try
					{
						// 添加 下行流量计数器
						DownloadCounter.Add(new PerformanceCounter(@"Network Interface", @"Bytes Received/sec", Instances[i]));
					}
					catch
					{
						// ignored
					}
				}
			}

			if (ListTool.HasElements(UploadCounter) && ListTool.HasElements(DownloadCounter))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// 启动流量监测
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public bool Start(int interval = 1000)
		{
			if (Init() && !IsStart)
			{
				DataCounterInterval = interval;
				IsStart = true;

				Task.Factory.StartNew(() =>
				{
					while (IsStart)
					{
						DataMonitorEvent?.Invoke(this);
						try
						{
							UploadDataCount += UploadData;
							UploadData = 0;
							foreach (var uc in UploadCounter)
							{
								UploadData += (int)uc?.NextValue();
							}
							DownloadDataCount += DownloadData;
							DownloadData = 0;
							foreach (var dc in DownloadCounter)
							{
								DownloadData += (int)dc?.NextValue();
							}
						}
						catch (Exception)
						{
							// ignored
						}

						Thread.Sleep(DataCounterInterval);
					}
				});
				return true;
			}
			return false;
		}
		/// <summary>
		/// 重启流量计数器
		/// </summary>
		public void Restart()
		{
			if (IsStart)
			{
				foreach (var uc in UploadCounter)
				{
					uc?.Close();
				}
				foreach (var dc in DownloadCounter)
				{
					dc?.Close();
				}
			}

			Init();
		}
		/// <summary>
		/// 重置流量表数
		/// </summary>
		public void Reset()
		{
			UploadData = 0;
			UploadDataCount = 0;
			DownloadData = 0;
			DownloadDataCount = 0;
		}
		/// <summary>
		/// 停止流量监测
		/// </summary>
		public void Stop()
		{
			if (IsStart)
			{
				IsStart = false;
				foreach (var uc in UploadCounter)
				{
					uc?.Close();
				}
				foreach (var dc in DownloadCounter)
				{
					dc?.Close();
				}
			}
		}
		/// <summary>
		/// 终结器
		/// </summary>
		~NetFlowTool()
		{
			Stop();
		}
	}
}