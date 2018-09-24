using System;
using System.Net;
using System.Windows.Forms;

namespace WebProxy
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			groupBox2.Enabled = radioButton3.Checked;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			textBox5.Text = GetHttpStatusCode(textBox4.Text, 3000);
			//textBox5.Text = GetHtml(textBox4.Text);
		}

		private string GetHttpStatusCode(string hostname, int timeout)
		{
			var request = (HttpWebRequest)WebRequest.Create(hostname);
			string status;
			var ip = string.Empty;
			try
			{
				request.Timeout = timeout;

				//获取服务端 IP
				IPEndPoint remoteEP = null;
				request.ServicePoint.BindIPEndPointDelegate = delegate (ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
				{
					remoteEP = remoteEndPoint;
					return null;
				};

				var response = (HttpWebResponse)request.GetResponse();
				
				var mystream = response.GetResponseStream();
				var myencoding = System.Text.Encoding.GetEncoding(response.CharacterSet);
				var mystreamreader = new System.IO.StreamReader(mystream, myencoding);
				var content = mystreamreader.ReadToEnd();

				response.Close();

				ip = remoteEP?.Address.ToString() ?? request.Address.Host;
				status = Convert.ToInt32(response.StatusCode).ToString();
			}
			catch (WebException ex)
			{
				var res = (HttpWebResponse)ex.Response;
				status = res == null ? ex.Message : Convert.ToInt32(res.StatusCode).ToString();
			}
			finally
			{
				request.Abort();
			}
			return status;
		}

	}
}
