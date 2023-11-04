using System.Diagnostics;
using Monitor.Models;
using Monitor.Models.StorageJSON;
using Newtonsoft.Json;

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
			Console.WriteLine(lines[i]);
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
					string idleString = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries).Skip(3).First();
					double idle = double.Parse(idleString.Split("id").First().Trim());
					double cpuUsage = (100 - idle) / 100;
					cpu.Usage = cpuUsage;
					break;
				}
				case >= 7:
				{
					string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					double cpuPercentage = double.Parse(parts[8]) / 100;
					if (cpuPercentage == 0) continue;
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
					if (memPercentage == 0) continue;
					memory.Processes.Add(new ProcessMemoryLog(command, memPercentage));
					break;
				}
			}
		}

		process.WaitForExit();
		return memory;
	}

	public static ServiceLog GetServiceInfo(string serviceName, DateTime? lastLogDate)
	{
		var service = new ServiceLog { Name = serviceName };
		var process = StartProcess("systemctl", $"status {serviceName}");

		//Getting service status, used memory, etc.
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

		//Getting service logs since the last time logging was executed
		string arguments = "";
		if (lastLogDate != null) arguments += $"--since=\"{lastLogDate:yyyy-MM-dd HH:mm:ss}\" ";
		arguments += $"--output=short-iso -u {serviceName}";
		process = StartProcess("journalctl", arguments);
		foreach (string logString in process.StandardOutput.ReadToEnd().Split('\n'))
		{
			int index = logString.IndexOf(" ", StringComparison.Ordinal);
			if (index == -1 || logString.StartsWith("--")) continue;
			var date = DateTime.Parse(logString[..index]);
			var log = new ServiceJournalLog(date, logString[(index + 1)..]);
			service.Logs.Add(log);
		}

		process.WaitForExit();
		return service;
	}

	public static IEnumerable<StorageLog> GetStorageInfo()
	{
		var process = StartProcess("lsblk", "-p -f -b --json");
		var jsonStorage = JsonConvert.DeserializeObject<StorageJson>(process.StandardOutput.ReadToEnd());
		if (jsonStorage == null) throw new Exception("Can't parse storage info!");

		foreach (var blockDevice in jsonStorage.BlockDevices)
		{
			//Going through every partition and returning it
			foreach (var child in blockDevice.Children)
			{
				if (child.Uuid == null) continue;
				string? mountpoint = blockDevice.Mountpoints.Count == 0 ? null : blockDevice.Mountpoints.First();
				long? fsAvail = blockDevice.FsAvail == null ? null : long.Parse(blockDevice.FsAvail);
				double? used = child.FsUse == null ? null : double.Parse(child.FsUse.Split('%').First()) / 100;
				yield return new StorageLog(child.Uuid, child.Label, child.FsType, child.FsVer, mountpoint, fsAvail, used);
			}
		}
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