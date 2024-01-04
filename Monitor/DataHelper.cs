using Newtonsoft.Json;
using YALM.Monitor.Models.CpuJSON;
using YALM.Monitor.Models.LogInfo;
using YALM.Monitor.Models.StorageJSON;

namespace YALM.Monitor;

public class DataHelper
{
	internal readonly RealTimeInfo RealTimeInfo = new();

	public async Task<CpuInfo?> GetCpuInfo()
	{
		var cpuInfo = new CpuInfo();

		//Getting cpu info from "top" command
		var process = ProcessHelper.StartProcess("top", "-bn1");
		string[] lines = (await process.StandardOutput.ReadToEndAsync()).Split('\n');
		foreach (string line in lines)
		{
			if (line.StartsWith("Tasks:"))
			{
				foreach (var value in ParseTopLine(line))
				{
					if (string.CompareOrdinal(value.Item1, "total") != 0) continue;
					cpuInfo.NumberOfTasks = (int)value.Item2;
					break;
				}
			}
			else if (line.StartsWith("%Cpu(s):"))
			{
				foreach (var value in ParseTopLine(line))
				{
					if (string.CompareOrdinal(value.Item1, "id") != 0) continue;
					cpuInfo.CpuUsage = (100 - value.Item2) / 100;
					break;
				}
				break;
			}
		}

		await process.WaitForExitAsync();

		//Getting additional cpu info from "lscpu" command
		process = ProcessHelper.StartProcess("lscpu", "-B -J");
		var cpuJson = JsonConvert.DeserializeObject<LscpuJson>(await process.StandardOutput.ReadToEndAsync());
		if (cpuJson?.Fields == null) throw new Exception("Invalid CPU info data!");
		foreach (var field in cpuJson.Fields)
		{
			switch (field.Field)
			{
				case "Architecture:":
					cpuInfo.Architecture = field.Data;
					break;
				case "CPU(s):":
					if (int.TryParse(field.Data, out int threads)) cpuInfo.Threads = threads;
					break;
				case "Model name:":
					cpuInfo.Name = field.Data;
					break;
				case "Core(s) per socket:":
					if (int.TryParse(field.Data, out int cores)) cpuInfo.Cores = cores;
					break;
				case "CPU max MHz:":
					if (double.TryParse(field.Data, out double frequency)) cpuInfo.Frequency = (int)frequency;
					break;
			}
		}

		await process.WaitForExitAsync();
		return cpuInfo;
	}

	public async Task<MemoryInfo?> GetMemoryInfo()
	{
		var memoryInfo = new MemoryInfo();

		//Getting memory information from the "top" command
		var process = ProcessHelper.StartProcess("top", "-bn1");
		string[] lines = (await process.StandardOutput.ReadToEndAsync()).Split('\n');
		foreach (string line in lines)
		{
			if (line.StartsWith("MiB Mem : "))
			{
				foreach (var value in ParseTopLine(line))
				{
					ulong kbValue = (ulong)(value.Item2 * 1024);
					switch (value.Item1)
					{
						case "total":
							memoryInfo.MemoryTotalKb = kbValue;
							break;
						case "free":
							memoryInfo.MemoryFreeKb = kbValue;
							break;
						case "used":
							memoryInfo.MemoryUsedKb = kbValue;
							break;
						case "buff/cache":
							memoryInfo.CachedKb = kbValue;
							break;
					}
				}
			}
			else if (line.StartsWith("MiB Swap:"))
			{
				foreach (var value in ParseTopLine(line))
				{
					ulong kbValue = (ulong)(value.Item2 * 1024);
					switch (value.Item1)
					{
						case "total":
							memoryInfo.SwapTotalKb = kbValue;
							break;
						case "free":
							memoryInfo.SwapFreeKb = kbValue;
							break;
						case "used":
							memoryInfo.SwapUsedKb = kbValue;
							break;
						case "Mem":
							memoryInfo.AvailableMemoryKb = kbValue;
							break;
					}
				}	
				break;
			}
		}

		await process.WaitForExitAsync();
		return memoryInfo;
	}

	public async Task<List<ProcessLog>?> GetProcessInfo()
	{
		var processLogs = new List<ProcessLog>();

		//Spawning "top" process and skipping first batch of data (top is printing data for 2 seconds, and first one is being ignored, for some reason the measurements are more correct this way)
		var process = ProcessHelper.StartProcess("top", "-bn2 --sort-override \"%CPU\" -w 400");
		var lines = (await process.StandardOutput.ReadToEndAsync()).Split('\n').ToList();
		int startIndex = lines.FindIndex(1, s => s.StartsWith("top -"));
		lines = lines.Skip(startIndex).ToList();

		//Parsing data from "top" process
		bool commandListStart = false;
		foreach (string line in lines)
		{
			if (line.Trim().StartsWith("PID"))
			{
				commandListStart = true;
			}
			else if (commandListStart)
			{
				if (line.Length == 0)
				{
					break;
				}

				string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				double cpuPercentage = double.Parse(parts[8]) / 100;
				double memPercentage = double.Parse(parts[9]) / 100;
				string command = parts[11];
				if (cpuPercentage == 0 || memPercentage == 0) continue;

				//Adding the process to the list of processes (or updating process CPU usage if it already exists)
				var proc = processLogs.FirstOrDefault(p => string.CompareOrdinal(p.Name, command) == 0);
				if (proc != null)
				{
					proc.CpuUsage += cpuPercentage;
					proc.MemoryUsage += memPercentage;
				}
				else
				{
					processLogs.Add(new ProcessLog { Name = command, CpuUsage = cpuPercentage, MemoryUsage = memPercentage });
				}
			}
		}
		
		await process.WaitForExitAsync();
		
		//A process will be stored only if it is top 10 cpu/memory using process
		var topCpuProcessLogs = processLogs.OrderByDescending(p => p.CpuUsage).Take(10).ToList();
		var topMemoryProcessLogs = processLogs.OrderByDescending(p => p.MemoryUsage).Take(10).ToList();
		processLogs = processLogs.Where(p => topCpuProcessLogs.Contains(p) || topMemoryProcessLogs.Contains(p)).ToList();
		return processLogs;
	}

	public ServiceLog GetServiceInfo(string serviceName, DateTime? lastLogDate)
	{
		var service = new ServiceLog { Name = serviceName };
		var process = ProcessHelper.StartProcess("systemctl", $"status {serviceName}");

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
		process = ProcessHelper.StartProcess("journalctl", arguments);
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

	public IEnumerable<StorageLog> GetStorageInfo()
	{
		var process = ProcessHelper.StartProcess("lsblk", "-p -f -b --json");
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
				yield return new StorageLog
				{
					Uuid = child.Uuid,
					Label = child.Label,
					Bytes = fsAvail != null && used != null ? (long)(fsAvail / used) : -1,
					FilesystemName = child.FsType,
					FilesystemVersion = child.FsVer,
					MountPath = mountpoint,
					UsedPercentage = used
				};
			}
		}
	}
	
	private IEnumerable<Tuple<string, double>> ParseTopLine(string line)
	{
		line = line.Replace("used.", "used,");
		foreach (string value in line.Split(new[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
		{
			string[] valueParts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			yield return new Tuple<string, double>(valueParts.Last(), double.Parse(valueParts.First().Trim()));
		}
	}
}