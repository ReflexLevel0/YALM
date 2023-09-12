using System.Diagnostics;
using System.Text.Json;

namespace Monitor;

internal class Monitor
{
	private static void Main()
	{
		var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
		if (config == null) throw new Exception("Configuration invalid!");
		var processInfo = new ProcessStartInfo("/bin/bash", "");

		Console.WriteLine($"Interval: {config.IntervalInSeconds}");
		
		if (config.Cpu)
		{
			Console.WriteLine("top -bn1 --sort-override \"%CPU\"");
		}
		
		if (config.Memory)
		{
			Console.WriteLine("top -bn1 --sort-override \"%MEM\"");
		}

		if (config.Storage)
		{
			Console.WriteLine("df -H");
		}

		if (config.Network)
		{
			//TODO: figure out a way to track network usage
			Console.WriteLine();
		}
		
		if (config.Services != null)
		{
			foreach (string service in config.Services)
			{
				Console.WriteLine($"systemctl status {service}");
			}
		}
	}
}