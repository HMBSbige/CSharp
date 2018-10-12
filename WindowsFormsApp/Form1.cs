using Common;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//HintTextBox
			HintTextBox.SetCueText(textBox1, @"Test hint text");
			//AdminButton
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

		#region AdminButton

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

		private void DoAdminTask()
		{
			try
			{
				var fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
							   @"\Microsoft\Windows\Start Menu\TEST.BG";
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

		#endregion


	}
}
