using Common;
using CommonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

			dataGridView1.DataSource = Table;
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

		#region 绑定显示列表，并排序后不影响更新数据

		private readonly BindingCollection<Data> Table = new BindingCollection<Data>();
		private ConcurrentList<Data> realtable = new ConcurrentList<Data>();

		private void button2_Click(object sender, EventArgs e)
		{
			for (var i = 0; i < 10; ++i)
			{
				var data = new Data
				{
						Index = i + 1
				};
				realtable.Add(data);
			}

			ToShowTable(realtable);
			button2.Enabled = false;
		}

		private void ToShowTable(IEnumerable<Data> table)
		{
			foreach (var item in table)
			{
				if (Table.Any(x => x.Index == item.Index))
				{
					//foreach (var x in Table)
					//{
					//	if (item.Index == x.Index)
					//	{
					//		x.Latency = item.Latency;
					//	}
					//}
				}
				else
				{
					Table.Add(item);
				}
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Task.Run(() =>
			{
				realtable[0].Latency = 100;
				Thread.Sleep(1000);
				realtable[1].Latency = 524;
				Thread.Sleep(1000);
				realtable[2].Latency = 244;
				Thread.Sleep(1000);
				realtable[3].Latency = 1450;
				Thread.Sleep(1000);
				realtable[4].Latency = 444;
				Thread.Sleep(1000);
				realtable[5].Latency = 666;
			});

		}

		private void button3_Click(object sender, EventArgs e)
		{
			var data = new Data
			{
					Index = realtable.Count + 1
			};
			realtable.Add(data);
			ToShowTable(realtable);
		}
		
		#endregion

	}
}
