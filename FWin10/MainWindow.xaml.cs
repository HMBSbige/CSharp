using System;
using System.Windows;
using System.Windows.Controls;
using Common;
using Microsoft.Win32;

namespace FWin10
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private const string Key1 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
		private const string Key2 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update";
		private const string Value1 = @"AUOptions";
		private const string Value2 = @"NoAutoUpdate";
		private const string Value3 = @"CachedAUOptions";
		private bool _isWin10;
		private void Reflesh()
		{
			if (_isWin10)
			{
				var type = Convert.ToInt32(Reg.Read(Key1, Value1));
				if (type <= 4 && type >= 1)
				{
					ChooseUpdateBox.SelectedIndex = type - 1;
				}
			}
		}

		private void RefleshButton_Click(object sender, RoutedEventArgs e)
		{
			Reflesh();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (Convert.ToDouble(WindowsVersion.GetOSSimpleVersion()) >= 6.4)
			{
				_isWin10 = true;
			}
			Reflesh();
		}

		private void ChooseUpdateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string key, value1, value2;
			if (_isWin10)
			{
				key = Key1;
				value1 = Value1;
				value2 = Value2;
				if (ChooseUpdateBox.SelectedIndex == 0)
				{
					Reg.Set(key, value1, 1, RegistryValueKind.DWord);
					Reg.Set(key, value2, 1, RegistryValueKind.DWord);
				}
				else if (ChooseUpdateBox.SelectedIndex == 1)
				{
					Reg.Set(key, value1, 2, RegistryValueKind.DWord);
					Reg.Set(key, value2, 1, RegistryValueKind.DWord);
				}
				else if (ChooseUpdateBox.SelectedIndex == 2)
				{
					Reg.Set(key, value1, 3, RegistryValueKind.DWord);
					Reg.Set(key, value2, 1, RegistryValueKind.DWord);
				}
				else if (ChooseUpdateBox.SelectedIndex == 3)
				{
					Reg.Set(key, value1, 4, RegistryValueKind.DWord);
					Reg.Set(key, value2, 0, RegistryValueKind.DWord);
				}
			}
			else
			{
				key = Key2;
				value1 = Value1;
				value2 = Value3;
			}
		}
	}
}
