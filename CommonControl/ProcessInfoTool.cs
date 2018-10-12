﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CommonControl
{
	public static class ProcessInfoTool
	{
		#region 程序集信息
		struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.LPStr)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string szTypeName;
			public SHFILEINFO(bool b)
			{
				hIcon = IntPtr.Zero;
				iIcon = 0;
				dwAttributes = 0u;
				szDisplayName = string.Empty;
				szTypeName = string.Empty;
			}
		}
		enum SHGFI
		{
			SmallIcon = 1,
			LargeIcon = 0,
			Icon = 256,
			DisplayName = 512,
			Typename = 1024,
			SysIconIndex = 16384,
			UseFileAttributes = 16
		}
		#endregion
		[DllImport(@"Shell32.dll")]
		static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);
		static Icon GetIcon(string file, bool small)
		{
			try
			{
				var sHFILEINFO = new SHFILEINFO(true);
				var cbfileInfo = Marshal.SizeOf(sHFILEINFO);
				SHGFI uFlags;
				if (small)
				{
					uFlags = (SHGFI)273;
				}
				else
				{
					uFlags = (SHGFI)272;
				}
				SHGetFileInfo(file, 256u, out sHFILEINFO, (uint)cbfileInfo, uFlags);
				return Icon.FromHandle(sHFILEINFO.hIcon);
			}
			catch
			{
				// ignored
			}

			return null;
		}
		public static Icon GetIcon(Process p, bool small)
		{
			try
			{
				var fileName = p.MainModule.FileName;
				return GetIcon(fileName, small);
			}
			catch
			{
				// ignored
			}

			return null;
		}
		[Obsolete]
		public static Icon GetIcon(int pid, bool small)
		{
			var p = Process.GetProcessById(pid);
			var ic = GetIcon(p, small);
			p.Close();
			return ic;
		}
		[Obsolete]
		public static string GetNameById(int pid)
		{
			var result = string.Empty;
			try
			{
				var processById = Process.GetProcessById(pid);
				result = processById.ProcessName;
				processById.Close();
			}
			catch (Exception)
			{
				// ignored
			}

			return result.Trim();
		}
	}
}
