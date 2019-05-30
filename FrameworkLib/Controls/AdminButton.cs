using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FrameworkLib.Controls
{
	public static class AdminButton
	{
		[DllImport(@"user32")]
		private static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

		private const int BCM_FIRST = 0x1600;
		private const int BCM_SETSHIELD = BCM_FIRST + 0x000C;

		public static bool IsVistaOrHigher()
		{
			return Environment.OSVersion.Version.Major < 6;
		}

		/// <summary>
		/// Add a shield icon to a button
		/// </summary>
		/// <param name="b">The button</param>
		public static void AddShieldToButton(Button b)
		{
			b.FlatStyle = FlatStyle.System;
			SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
		}

		/// <summary>
		/// Restart the current process with administrator credentials
		/// </summary>
		public static void RestartElevated()
		{
			var startInfo = new ProcessStartInfo
			{
				UseShellExecute = true,
				WorkingDirectory = Environment.CurrentDirectory,
				FileName = Application.ExecutablePath,
				Verb = @"runas"
			};
			try
			{
				Process.Start(startInfo);
			}
			catch (System.ComponentModel.Win32Exception)
			{
				return; //If cancelled, do nothing
			}

			Application.Exit();
		}
	}
}