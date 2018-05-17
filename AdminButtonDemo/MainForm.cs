using System;
using System.Windows.Forms;
using Common;

namespace AdminButtonDemo
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			if (!AdminButton.IsAdmin())
			{
				Text += @"（标准用户权限）";
				AdminButton.AddShieldToButton(button1);
			}
			else
			{
				Text += @"（管理员权限）";
			}
		}

		public sealed override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}

		private void DoAdminTask()
		{
			try
			{
				var fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
							   + @"\Microsoft\Windows\Start Menu\TEST.BG";
				if (System.IO.File.Exists(fileName))
				{
					System.IO.File.Delete(fileName);
					MessageBox.Show(@"TEST.BG 删除成功");
				}
				else
				{
					System.IO.File.Create(fileName).Close();
					MessageBox.Show(@"TEST.BG 创建成功");
				}
			}
			catch (UnauthorizedAccessException uacEx)
			{
				MessageBox.Show(uacEx.Message, uacEx.Data.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.Data.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (AdminButton.IsAdmin())
			{
				DoAdminTask();
			}
			else
			{
				AdminButton.RestartElevated();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DoAdminTask();
		}
	}
}
