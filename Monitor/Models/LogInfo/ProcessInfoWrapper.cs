namespace YALM.Monitor.Models.LogInfo;

public class ProcessInfoWrapper : ProcessInfo
{
	public DateTime? LastRefreshedDateTime { get; set; }

	public async Task RefreshProcessInfo()
	{
		if (LastRefreshedDateTime != null && DateTime.Now.Subtract((DateTime)LastRefreshedDateTime).TotalMinutes < 1) return;
		
		//Spawning "top" process and skipping first batch of data (top is printing data for 2 seconds, and first one is being ignored, for some reason the measurements are more correct this way)
		var process = ProcessHelper.StartProcess("top", "-bn2 -w 400");
		var lines = (await process.StandardOutput.ReadToEndAsync()).Split('\n').ToList();
		int startIndex = lines.FindIndex(1, s => s.StartsWith("top -"));
		lines = lines.Skip(startIndex).ToList();

		//Parsing data from "top" process
		bool commandListStart = false;
		CpuLog = new CpuLog();
		MemoryLog = new MemoryLog();
		foreach (string line in lines)
		{
			if (line.StartsWith("Tasks:"))
			{
				foreach (var value in ParseTopLine(line))
				{
					if (string.CompareOrdinal(value.Item1, "total") != 0) continue;
					CpuLog.NumberOfTasks = (int)value.Item2;
					break;
				}
			}
			else if (line.StartsWith("%Cpu(s):"))
			{
				foreach (var value in ParseTopLine(line))
				{
					if (string.CompareOrdinal(value.Item1, "id") != 0) continue;
					CpuLog.Usage = (100 - value.Item2) / 100;
					break;
				}
			}
			else if (line.StartsWith("MiB Mem : "))
			{
				foreach (var value in ParseTopLine(line))
				{
					ulong kbValue = (ulong)(value.Item2 * 1024);
					switch (value.Item1)
					{
						case "total":
							MemoryLog.MemoryTotalKb = kbValue;
							break;
						case "free":
							MemoryLog.MemoryFreeKb = kbValue;
							break;
						case "used":
							MemoryLog.MemoryUsedKb = kbValue;
							break;
						case "buff/cache":
							MemoryLog.CachedKb = kbValue;
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
							MemoryLog.SwapTotalKb = kbValue;
							break;
						case "free":
							MemoryLog.SwapFreeKb = kbValue;
							break;
						case "used":
							MemoryLog.SwapUsedKb = kbValue;
							break;
						case "Mem":
							MemoryLog.AvailableMemoryKb = kbValue;
							break;
					}
				}
			}
			else if (line.Trim().StartsWith("PID"))
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
		LastRefreshedDateTime = DateTime.Now;
		
		//A process will be stored only if it is top 10 cpu/memory using process
		var topCpuProcessLogs = ProcessLogs.OrderByDescending(p => p.CpuUsage).Take(10).ToList();
		var topMemoryProcessLogs = ProcessLogs.OrderByDescending(p => p.MemoryUsage).Take(10).ToList();
		ProcessLogs = ProcessLogs.Where(p => topCpuProcessLogs.Contains(p) || topMemoryProcessLogs.Contains(p)).ToList();
	}
	
	private static IEnumerable<Tuple<string, double>> ParseTopLine(string line)
	{
		line = line.Replace("used.", "used,");
		foreach (string value in line.Split(new[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
		{
			string[] valueParts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			yield return new Tuple<string, double>(valueParts.Last(), double.Parse(valueParts.First().Trim()));
		}
	}
}