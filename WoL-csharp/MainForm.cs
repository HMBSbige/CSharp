using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WoL_csharp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private const string MacCheckRegexString = @"^([0-9a-fA-F]{2})(([/\s:-][0-9a-fA-F]{2}){5})$";

		private static readonly Regex MacCheckRegex = new Regex(MacCheckRegexString);

		private static void WakeUp(string mac, string ip)
		{
			byte[] macByte;
			try
			{
				macByte = FormatMac(mac);
			}
			catch
			{
				macByte = FormatMac2(mac);
			}
			var broadcast = IPAddress.Parse(ip);
			WakeUpCore(macByte, broadcast);
		}

		private static void WakeUpCore(IReadOnlyList<byte> mac, IPAddress broadcast)
		{
			//https://en.wikipedia.org/wiki/Wake-on-LAN
			var client = new UdpClient();
			client.Connect(broadcast, 9090);
			var packet = new byte[17 * 6];
			for (var i = 0; i < 6; ++i)
				packet[i] = 0xFF;
			for (var i = 1; i <= 16; ++i)
			{
				for (var j = 0; j < 6; ++j)
				{
					packet[i * 6 + j] = mac[j];
				}
			}
			client.Send(packet, packet.Length);
		}

		private static byte[] FormatMac(string macInput)
		{
			var mac = new byte[6];
			var str = macInput;
			var sArray = str.Split('-');
			for (var i = 0; i < 6; ++i)
			{
				var byteValue = Convert.ToByte(sArray[i], 16);
				mac[i] = byteValue;
			}
			return mac;
		}

		private static byte[] FormatMac2(string macInput)
		{
			var mac = new byte[6];
			var str = macInput;
			var sArray = str.Split(':');
			for (var i = 0; i < 6; ++i)
			{
				var byteValue = Convert.ToByte(sArray[i], 16);
				mac[i] = byteValue;
			}
			return mac;
		}

		/// <summary>
		/// 判断字符串是否为点分十进制IPv4地址，如x.x.x.x
		/// </summary>
		/// <param name="strIP">需判断的字符串</param>
		/// <returns>如果是返回真，否则返回假</returns>
		private static bool IsIPv4(ref string strIP)
		{
			try
			{
				strIP = strIP.Trim();
				var tempIP = IPAddress.Parse(strIP);
				return tempIP.ToString() == strIP && tempIP.AddressFamily == AddressFamily.InterNetwork;
			}
			catch
			{
				return false;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var mac = textBox1.Text;
			var ip = textBox2.Text;
			if (!MacCheckRegex.IsMatch(mac))
			{
				toolStripStatusLabel1.Text = @"Mac 地址格式错误！";
				return;
			}
			if (!IsIPv4(ref ip))
			{
				toolStripStatusLabel1.Text = @"广播地址格式错误！";
				return;
			}
			try
			{
				WakeUp(mac, ip);
			}
			catch (Exception ex)
			{
				toolStripStatusLabel1.Text = @"发送网络唤醒失败！";
				MessageBox.Show(ex.Message, ex.Data.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			toolStripStatusLabel1.Text = @"发送网络唤醒成功！";
		}
	}
}
