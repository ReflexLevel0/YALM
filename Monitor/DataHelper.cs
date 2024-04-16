using Newtonsoft.Json;
using YALM.Monitor.Exceptions;
using YALM.Monitor.Models.CpuJSON;
using YALM.Monitor.Models.LogInfo;

namespace YALM.Monitor;

public class DataHelper
{
	private readonly ProgramInfoWrapper _programInfoWrapper = new();
	
	public async Task<CpuInfo?> GetCpuInfo()
	{
		var cpuInfo = new CpuInfo();
		
		//Getting additional cpu info from "lscpu" command
		var process = await ProcessHelper.StartProcess("lscpu", "-B -J");
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

	public async Task<ProgramInfo> GetProgramInfo()
	{
		await _programInfoWrapper.RefreshProgramInfo();
		return _programInfoWrapper;
	}

	/// <summary>
	/// Gets information about service <paramref name="serviceName"/>
	/// </summary>
	/// <param name="serviceName">Name of the service whose status is being returned</param>
	/// <param name="lastLogDate">Last time data about service was logged</param>
	/// <returns></returns>
	/// <exception cref="ServiceNotFoundException"></exception>
	public async Task<ServiceLog> GetServiceInfo(string serviceName, DateTime? lastLogDate)
	{
		var service = new ServiceLog { Name = serviceName };
		var process = await ProcessHelper.StartProcess("systemctl", $"status {serviceName}");
		var lines = (await process.StandardOutput.ReadToEndAsync()).Split('\n').Take(17).ToList();
		string error = await process.StandardError.ReadToEndAsync();
		if(string.IsNullOrWhiteSpace(error) == false) throw new ServiceNotFoundException(serviceName);
		
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

		await process.WaitForExitAsync();

		//Getting service logs since the last time logging was executed
		string arguments = "";
		if (lastLogDate != null) arguments += $"--since=\"{lastLogDate:yyyy-MM-dd HH:mm:ss}\" ";
		arguments += $"--output=short-iso -u {serviceName}";
		process = await ProcessHelper.StartProcess("journalctl", arguments);
		foreach (string logString in (await process.StandardOutput.ReadToEndAsync()).Split('\n'))
		{
			int index = logString.IndexOf(" ", StringComparison.Ordinal);
			if (index == -1 || logString.StartsWith("--")) continue;
			var date = DateTime.Parse(logString[..index]);
			var log = new ServiceJournalLog(date, logString[(index + 1)..]);
			service.Logs.Add(log);
		}

		await process.WaitForExitAsync();
		return service;
	}

	public async Task<List<DiskInfo>> GetStorageInfo()
	{
		var process = await ProcessHelper.StartProcess("lsblk", "--json -p -b -o FSTYPE,FSUSED,LABEL,NAME,PARTTYPENAME,PARTUUID,PATH,PTTYPE,PTUUID,SERIAL,SIZE,STATE,MOUNTPOINT,TYPE,UUID,VENDOR,FSVER,FSAVAIL,FSUSE%");
		var jsonStorage = JsonConvert.DeserializeObject<StorageJson>(await process.StandardOutput.ReadToEndAsync());
		if (jsonStorage == null) throw new Exception("Can't parse storage info!");
		return jsonStorage.BlockDevices;
	}
}