using System.Diagnostics;

namespace Monitor;

public class DataHelper
{
	public static Cpu GetCpuInfo()
	{
		var cpu = new Cpu();
		var process = new Process();
		process.StartInfo.FileName = "/usr/bin/top";
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.Arguments = "-bn1 --sort-override \"%CPU\" -w 400";
		process.Start();
		
		var lines = process.StandardOutput.ReadToEnd().Split('\n').Take(17).ToList();
		for (int i = 0; i < lines.Count; i++)
		{
			switch (i)
			{
				case 1:
				{
					string totalTasks = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).First();
					Console.WriteLine($"Total number of tasks: {totalTasks}");
					break;
				}
				case 2:
				{
					double cpuUsage = (100 - double.Parse(lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(7).First())) / 100;
					cpu.Usage = cpuUsage;
					Console.WriteLine($"CPU usage: {cpu.Usage:P}");
					break;
				}
				case >= 7:
				{
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					double cpuPercentage = double.Parse(parts[8]) / 100;
					string command = parts[11];
					cpu.Processes.Add(new ProcessCpuInfo(command, cpuPercentage));
					Console.WriteLine($"{command} {cpuPercentage:P} CPU");
					break;
				}
			}
		}

		return cpu;
	}

	public static Memory GetMemoryInfo()
	{
		var memory = new Memory();
		var process = new Process();
		process.StartInfo.FileName = "/usr/bin/top";
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.Arguments = "-bn1 --sort-override \"%MEM\" -w 400";
		process.Start();
		
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
					Console.WriteLine($"Memory: {totalMemory}MB total, {usedMemory}MB used, {usage:P} used");
					break;
				}
				case >= 7:
				{
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					string memPercentage = parts[9];
					string command = parts[11];
					Console.WriteLine($"{command} {memPercentage}% MEM");
					break;
				}
			}
		}
		process.WaitForExit();

		return memory;
	}
}