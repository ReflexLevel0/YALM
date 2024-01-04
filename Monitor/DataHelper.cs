using Newtonsoft.Json;
using YALM.Monitor.Models;
using YALM.Monitor.Models.LogInfo;
using YALM.Monitor.Models.StorageJSON;

namespace YALM.Monitor;

public class DataHelper
{
	internal readonly RealTimeInfo RealTimeInfo = new();

	public async Task<CpuInfo?> GetCpuInfo()
	{
		await RealTimeInfo.RefreshCpuInfo();
		return RealTimeInfo.CpuInfo;
	}

	public async Task<MemoryInfo?> GetMemoryInfo()
	{
		await RealTimeInfo.RefreshMemoryInfo();
		return RealTimeInfo.MemoryInfo;
	}

	public async Task<List<ProcessLog>?> GetProcessInfo()
	{
		await RealTimeInfo.RefreshProcessInfo();
		return RealTimeInfo.ProcessLogs?.ToList();
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
}