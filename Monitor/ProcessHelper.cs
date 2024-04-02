using System.Diagnostics;

namespace YALM.Monitor;

public class ProcessHelper
{
	public static async Task<Process> StartProcess(string programPath, string arguments)
	{
		var process = new Process();
		
		//Checking if the program can be found on the system
		process.StartInfo.FileName = "which";
		process.StartInfo.Arguments = programPath;
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();
		string path = await process.StandardOutput.ReadToEndAsync();
		if (string.IsNullOrEmpty(path)) throw new Exception($"Program {programPath} not found on the system!");
		await process.WaitForExitAsync();

		//Starting the process
		process = new Process();
		process.StartInfo.FileName = programPath;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.Arguments = arguments;
		process.Start();
		return process;
	}
}