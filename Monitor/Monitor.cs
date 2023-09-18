using System.Diagnostics;
using System.Text.Json;

namespace Monitor;

internal class Monitor
{
	private static void Main()
	{
		var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		var process = new Process();
		process.StartInfo.FileName = "/usr/bin/top";
		process.StartInfo.RedirectStandardOutput = true;

		Console.WriteLine($"Interval: {config.IntervalInSeconds}\n");
		
		//Short sleep is necessary so that the program doesn't take up all the CPU and displays wrong statistics
		Thread.Sleep(2000);
		
		if (config.Cpu)
		{
			var cpu = LogHelper.GetCpuInfo();
			Console.WriteLine(cpu);
		}
		
		//Parsing memory information
		if (config.Memory)
		{
			var memory = LogHelper.GetMemoryInfo();
			Console.WriteLine(memory);
		}

		if (config.Storage)
		{
			Console.WriteLine("df -H");
		}

		if (config.Network)
		{
			//TODO: figure out a way to track network usage (this might require sudo privileges)
			Console.WriteLine();
		}
		
		if (config.Services != null)
		{
			foreach (string serviceName in config.Services)
			{
				var service = LogHelper.GetServiceInfo(serviceName);
				Console.WriteLine(service);
			}
		}
	}
}