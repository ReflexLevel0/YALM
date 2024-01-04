using System.Diagnostics;

namespace YALM.Monitor;

public class ProcessHelper
{
	public static Process StartProcess(string programPath, string arguments)
	{
		var process = new Process();
		process.StartInfo.FileName = programPath;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.Arguments = arguments;
		process.Start();
		return process;
	}
}