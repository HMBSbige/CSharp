using Common;
using CommonControl;
using System;
using System.Windows.Forms;

namespace WindowsFormsApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			UpdateStyles();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//HintTextBox
			HintTextBox.SetCueText(textBox1, @"Test hint text");
			//AdminButton
			if (!CheckPermission.IsAdmin())
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
			if (CheckPermission.IsAdmin())
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
				var fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Windows\Start Menu\TEST.BG";
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

		BindingCollection<Data> table = new BindingCollection<Data>();

		private void button2_Click(object sender, EventArgs e)
		{
			var data = new Data();
			table.Add(data);

			dataGridView1.DataSource = table;

		}

		private void button3_Click(object sender, EventArgs e)
		{
			for (var i = 0; i < table.Count; ++i)
			{
				table[i].Index = i + 1;
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			
			for (int i = 0; i < dataGridView1.Columns.Count; ++i)
			{
				dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
			}

			foreach (var item in table)
			{
				if (item.Index == 2)
				{
					item.Latency = 100;
					break;
				}
			}

			for (int i = 0; i < dataGridView1.Columns.Count; ++i)
			{
				dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
			}

		}

		private void button5_Click(object sender, EventArgs e)
		{
			
		}
	}
}
