using System.Diagnostics;

namespace Monitor;

public class DataParser
{
	public static CpuLog GetCpuInfo()
	{
		var cpu = new CpuLog();
		var process = StartProcess("top", "-bn1 --sort-override \"%CPU\" -w 400");

		var lines = process.StandardOutput.ReadToEnd().Split('\n').Take(17).ToList();
		for (int i = 0; i < lines.Count; i++)
		{
			switch (i)
			{
				case 1:
				{
					string totalTasks = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).First();
					cpu.NumberOfTasks = int.Parse(totalTasks);
					break;
				}
				case 2:
				{
					double cpuUsage = (100 - double.Parse(lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(7).First())) / 100;
					cpu.Usage = cpuUsage;
					break;
				}
				case >= 7:
				{
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					double cpuPercentage = double.Parse(parts[8]) / 100;
					if(cpuPercentage == 0) continue;
					string command = parts[11];
					cpu.Processes.Add(new ProcessCpuLog(command, cpuPercentage));
					break;
				}
			}
		}

		process.WaitForExit();
		return cpu;
	}

	public static MemoryLog GetMemoryInfo()
	{
		var memory = new MemoryLog();
		var process = StartProcess("top", "-bn1 --sort-override \"%MEM\" -w 400");

		var lines = process.StandardOutput.ReadToEnd().Split('\n').Take(17).ToList();
		for (int i = 0; i < lines.Count; i++)
		{
			switch (i)
			{
				case 3:
				{
					//TODO: take into account swap memory
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					double totalMemory = double.Parse(parts[3]);
					double usedMemory = double.Parse(parts[7]);
					double usage = usedMemory / totalMemory;
					memory.TotalMemoryMb = totalMemory;
					memory.UsedMemoryMb = usedMemory;
					memory.UsedPercentage = usage;
					break;
				}
				case >= 7:
				{
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					double memPercentage = double.Parse(parts[9]) / 100;
					string command = parts[11];
					if(memPercentage == 0) continue;
					memory.Processes.Add(new ProcessMemoryLog(command, memPercentage));
					break;
				}
			}
		}

		process.WaitForExit();
		return memory;
	}

	public static ServiceLog GetServiceInfo(string serviceName)
	{
		var service = new ServiceLog(serviceName);
		var process = StartProcess("systemctl", $"status {serviceName}");
		
		var lines = process.StandardOutput.ReadToEnd().Split('\n').Take(17).ToList();
		foreach (string line in lines)
		{
			string trimmedLine = line.Trim();
			if (string.IsNullOrWhiteSpace(trimmedLine))
			{
				break;
			}

			if (trimmedLine.StartsWith("Active:"))
			{
				service.Active = trimmedLine.Split("Active: ").Last();
			}
			else if (trimmedLine.StartsWith("Tasks:"))
			{
				service.Tasks = int.Parse(trimmedLine.Split("Tasks: ").Last().Split(" ").First());
			}
			else if (trimmedLine.StartsWith("Memory:"))
			{
				service.Memory = trimmedLine.Split("Memory: ").Last();
			}
			else if (trimmedLine.StartsWith("CPU:"))
			{
				service.Cpu = trimmedLine.Split("CPU: ").Last();
			}
		}

		process.WaitForExit();
		process = StartProcess("journalctl", $"--since=\"2023-09-18 00:00:00\" --output=short-iso -u {serviceName}");
		foreach (string logString in process.StandardOutput.ReadToEnd().Split('\n'))
		{
			int index = logString.IndexOf(" ", StringComparison.Ordinal);
			if (index == -1 || logString.StartsWith("--")) continue;
			var date = DateTime.Parse(logString[..index]);
			var log = new JournalLog(date, logString[(index + 1)..]);
			service.Logs.Add(log);
		}

		process.WaitForExit();
		return service;
	}

	private static Process StartProcess(string programPath, string arguments)
	{
		var process = new Process();
		process.StartInfo.FileName = programPath;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.Arguments = arguments;
		process.Start();
		return process;
	}
}