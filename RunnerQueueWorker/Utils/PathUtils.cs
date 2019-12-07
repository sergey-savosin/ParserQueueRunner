using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RunnerQueueWorker.Utils
{
	public static class PathUtils
	{
		public static string GetStartupPath()
		{
			return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		}

		public static string GetAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}

		public static string GetApplicationIniPath()
		{
			string startupPath = GetStartupPath();
			string EXE = GetAssemblyName();

			return Path.Combine(startupPath, EXE + ".ini");
		}
	}
}
