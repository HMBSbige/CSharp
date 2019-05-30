using FrameworkLib.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

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

		#region RegeditData
		private const string Win10UpdateKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
		private const string OldWinUpdateKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update";
		private const string DisableOneDriveKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive";
		private const string DisableCortanaKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search";
		private const string DisableWindowsDefenderKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender";
		private const string DisableCustomFolderKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer";
		private const string ExcludeWUDriversKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";
		private const string ExcludeMRTKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\MRT";

		private const string WinUpdateValue = @"AUOptions";
		private const string Win10AutoUpdateValue = @"NoAutoUpdate";
		private const string OldWinUpdateValue = @"CachedAUOptions";
		private const string AutoInstallUpdatesValue = @"AutoInstallMinorUpdates";
		private const string NoAutoRebootValue = @"NoAutoRebootWithLoggedOnUsers";
		private const string DisableOneDriveValue = @"DisableFileSyncNGSC";
		private const string AllowCortanaValue = @"AllowCortana";
		private const string DisableWindowsDefenderValue = @"DisableAntiSpyware";
		private const string DisableCustomFolderValue = @"NoCustomizeWebView";
		private const string ExcludeWUDriversValue = @"ExcludeWUDriversInQualityUpdate";
		private const string ExcludeMRTValue = @"DontOfferThroughWUAU";
		#endregion

		private bool _isWin10;
		private void Reflesh()
		{
			WindowsVersionLabel.Content = string.Format(@"版本：{0}", WindowsVersion.GetOSCompleteVersion());
			//Windows Update
			if (!_isWin10 && !Reg.Exist(Win10UpdateKey, WinUpdateValue))
			{
				ChooseUpdateBox.SelectedIndex = 0;
			}
			else
			{
				var type = Convert.ToInt32(Reg.Read(Win10UpdateKey, WinUpdateValue));//如果键值不存在 type==0
				if (type <= 4 && type >= 1)
				{
					ChooseUpdateBox.SelectedIndex = type - 1;
				}
				/*else
				{
					ChooseUpdateBox.IsEnabled = false;
				}*/
			}
			//Auto Install Updates
			AutoInstallUpdates_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(Win10UpdateKey, AutoInstallUpdatesValue)) == 1;
			//No Auto Reboot
			NoAutoReboot_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(Win10UpdateKey, NoAutoRebootValue)) == 1;
			//Disable OneDrive
			DisableOneDrive_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(DisableOneDriveKey, DisableOneDriveValue)) == 1;
			//Allow Cortana
			DisableCortana_CheckBox.IsChecked = Reg.Exist(DisableCortanaKey, AllowCortanaValue) && Convert.ToInt32(Reg.Read(DisableCortanaKey, AllowCortanaValue)) == 0;
			//Disable Windows Defender
			DisableWindowsDefender_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(DisableWindowsDefenderKey, DisableWindowsDefenderValue)) == 1;
			//Disable Custom Folder
			DisableCustomFolder_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(DisableCustomFolderKey, DisableCustomFolderValue)) == 1;
			//Exclude Drivers
			if (WindowsVersion.GetOSCompleteVersion() >= Version.Parse(@"10.0.14328.1000"))
			{
				ExcludeWUDrivers_CheckBox.IsEnabled = true;
				ExcludeWUDrivers_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(ExcludeWUDriversKey, ExcludeWUDriversValue)) == 1;
			}
			else
			{
				ExcludeWUDrivers_CheckBox.IsEnabled = false;
				ExcludeWUDrivers_CheckBox.IsChecked = false;
			}
			//Exclude MRT
			ExcludeMRT_CheckBox.IsChecked = Convert.ToInt32(Reg.Read(ExcludeMRTKey, ExcludeMRTValue)) == 1;
		}

		private void RefleshButton_Click(object sender, RoutedEventArgs e)
		{
			Reflesh();
		}

		private void RestartExplorerButton_Click(object sender, RoutedEventArgs e)
		{
			WinProcess.Stop(@"explorer");
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (WindowsVersion.GetOSCompleteVersion() >= Version.Parse(@"6.4"))
			{
				_isWin10 = true;
			}
			Reflesh();
		}

		#region StatusChange

		private void ChooseUpdateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string key, value1, value2;
			if (_isWin10)
			{
				key = Win10UpdateKey;
				value1 = WinUpdateValue;
				value2 = Win10AutoUpdateValue;
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
				key = OldWinUpdateKey;
				value1 = WinUpdateValue;
				value2 = OldWinUpdateValue;
				Reg.Set(key, value1, ChooseUpdateBox.SelectedIndex + 1, RegistryValueKind.DWord);
				Reg.Set(key, value2, ChooseUpdateBox.SelectedIndex + 1, RegistryValueKind.DWord);
			}
		}

		private void AutoInstallUpdates_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (AutoInstallUpdates_CheckBox.IsChecked == true)
			{
				Reg.Set(Win10UpdateKey, AutoInstallUpdatesValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Set(Win10UpdateKey, AutoInstallUpdatesValue, 0, RegistryValueKind.DWord);
			}
		}

		private void NoAutoReboot_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (NoAutoReboot_CheckBox.IsChecked == true)
			{
				Reg.Set(Win10UpdateKey, NoAutoRebootValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Set(Win10UpdateKey, NoAutoRebootValue, 0, RegistryValueKind.DWord);
			}
		}

		private void DisableOneDrive_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (DisableOneDrive_CheckBox.IsChecked == true)
			{
				Reg.Set(DisableOneDriveKey, DisableOneDriveValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Set(DisableOneDriveKey, DisableOneDriveValue, 0, RegistryValueKind.DWord);
			}
		}

		private void DisableCortana_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (DisableCortana_CheckBox.IsChecked == true)
			{
				Reg.Set(DisableCortanaKey, AllowCortanaValue, 0, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Delete(DisableCortanaKey, AllowCortanaValue);
			}
		}

		private void DisableWindowsDefender_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (DisableWindowsDefender_CheckBox.IsChecked == true)
			{
				Reg.Set(DisableWindowsDefenderKey, DisableWindowsDefenderValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Delete(DisableWindowsDefenderKey, DisableWindowsDefenderValue);
			}
		}

		private void DisableCustomFolder_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (DisableCustomFolder_CheckBox.IsChecked == true)
			{
				Reg.Set(DisableCustomFolderKey, DisableCustomFolderValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Delete(DisableCustomFolderKey, DisableCustomFolderValue);
			}
		}

		private void ExcludeWUDrivers_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (ExcludeWUDrivers_CheckBox.IsChecked == true)
			{
				Reg.Set(ExcludeWUDriversKey, ExcludeWUDriversValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Delete(ExcludeWUDriversKey, ExcludeWUDriversValue);
			}
		}

		private void ExcludeMRT_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (ExcludeMRT_CheckBox.IsChecked == true)
			{
				Reg.Set(ExcludeMRTKey, ExcludeMRTValue, 1, RegistryValueKind.DWord);
			}
			else
			{
				Reg.Delete(ExcludeMRTKey, ExcludeMRTValue);
			}
		}

		#endregion
	}
}
