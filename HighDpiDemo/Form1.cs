using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
#pragma warning disable 612

namespace HighDpiDemo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_DpiChanged(object sender, DpiChangedEventArgs e)
		{
			Debug.WriteLine($@"Dpi Changed:{e.DeviceDpiOld / 96.0 * 100}% => {e.DeviceDpiNew / 96.0 * 100}");
			button1.PerformClick();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//ChangeFormDpi(this);
			button1.PerformClick();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DpiUtils.SetDPIAwareOld();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			DpiUtils.ChangeFormDpi(this, 2.0);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			label6.Text = $@"Form DPI:{DpiUtils.GetFormDpi(this) * 100}%";
			label7.Text = $@"Device DPI:{this.GetDeviceDpi() * 100}%";
			label10.Text = $@"Check Environment:{DpiUtils.CheckHighDpiEnvironment()}";
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DpiUtils.ChangeFormDpi(this, 1.75);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < Screen.AllScreens.Length; i++)
			{
				var screen = Screen.AllScreens[i];
				screen.GetScreenDpi(DpiUtils.DpiType.Effective, out var x, out var y);
				sb.AppendLine($@"Screen{i+1}:{x / 96.0 * 100}%");
			}

			MessageBox.Show(sb.ToString());
		}
	}
}
