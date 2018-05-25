using System.Diagnostics;

namespace Common
{
	class WinProcess
	{
		public static void Stop(string processname)
		{
			foreach (var exe in Process.GetProcesses())
			{
				if (exe.ProcessName == processname)
				{
					exe.Kill();
					exe.WaitForExit();
					return;
				}
			}
		}

		public static void Start(string processname)
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = processname
			};
			Process.Start(startInfo);
		}

		public static void Restart(string processname)
		{
			Stop(processname);
			Start(processname);
		}
	}
}
