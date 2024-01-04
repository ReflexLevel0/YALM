using Newtonsoft.Json;
using YALM.Monitor.Models.CpuJSON;
using YALM.Monitor.Models.LogInfo;

namespace YALM.Monitor;

public class RealTimeInfo
{
	public CpuInfo? CpuInfo { get; set; }
	public MemoryInfo? MemoryInfo { get; set; }
	public List<ProcessLog>? ProcessLogs { get; set; }

	public async Task RefreshCpuInfo()
	{
		CpuInfo = new CpuInfo();

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
					CpuInfo.NumberOfTasks = (int)value.Item2;
					break;
				}
			}
			else if (line.StartsWith("%Cpu(s):"))
			{
				foreach (var value in ParseTopLine(line))
				{
					if (string.CompareOrdinal(value.Item1, "id") != 0) continue;
					CpuInfo.CpuUsage = (100 - value.Item2) / 100;
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
					CpuInfo.Architecture = field.Data;
					break;
				case "CPU(s):":
					if (int.TryParse(field.Data, out int threads)) CpuInfo.Threads = threads;
					break;
				case "Model name:":
					CpuInfo.Name = field.Data;
					break;
				case "Core(s) per socket:":
					if (int.TryParse(field.Data, out int cores)) CpuInfo.Cores = cores;
					break;
				case "CPU max MHz:":
					if (double.TryParse(field.Data, out double frequency)) CpuInfo.Frequency = (int)frequency;
					break;
			}
		}

		await process.WaitForExitAsync();
	}

	public async Task RefreshMemoryInfo()
	{
		MemoryInfo = new MemoryInfo();

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
							MemoryInfo.MemoryTotalKb = kbValue;
							break;
						case "free":
							MemoryInfo.MemoryFreeKb = kbValue;
							break;
						case "used":
							MemoryInfo.MemoryUsedKb = kbValue;
							break;
						case "buff/cache":
							MemoryInfo.CachedKb = kbValue;
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
							MemoryInfo.SwapTotalKb = kbValue;
							break;
						case "free":
							MemoryInfo.SwapFreeKb = kbValue;
							break;
						case "used":
							MemoryInfo.SwapUsedKb = kbValue;
							break;
						case "Mem":
							MemoryInfo.AvailableMemoryKb = kbValue;
							break;
					}
				}	
				break;
			}
		}

		await process.WaitForExitAsync();
	}

	public async Task RefreshProcessInfo()
	{
		ProcessLogs = new List<ProcessLog>();

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
				var proc = ProcessLogs.FirstOrDefault(p => string.CompareOrdinal(p.Name, command) == 0);
				if (proc != null)
				{
					proc.CpuUsage += cpuPercentage;
					proc.MemoryUsage += memPercentage;
				}
				else
				{
					ProcessLogs.Add(new ProcessLog { Name = command, CpuUsage = cpuPercentage, MemoryUsage = memPercentage });
				}
			}
		}
		
		await process.WaitForExitAsync();
		
		//A process will be stored only if it is top 10 cpu/memory using process
		var tempProcessList = ProcessLogs.OrderByDescending(p => p.CpuUsage).Take(10).ToList();
		tempProcessList.AddRange(ProcessLogs.Where(p => tempProcessList.Contains(p) == false).OrderByDescending(p => p.MemoryUsage).Take(10).ToList());
		ProcessLogs = tempProcessList;
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