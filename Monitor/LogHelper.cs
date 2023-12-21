using YALM.Monitor.Models;

namespace YALM.Monitor;

public class LogHelper
{
	private readonly Config _config;
	private DateTime? _lastLogDate;
	private const string LastLogDateFilename = "log_date.txt";

	public LogHelper(Config config)
	{
		_config = config;
		if (File.Exists(LastLogDateFilename))
		{
			_lastLogDate = DateTime.Parse(File.ReadAllText(LastLogDateFilename));
		}
	}
	
	public LogBase Log()
	{
		var log = new LogBase(DateTime.Now);

		if (_config.Cpu)
		{
			log.CpuLog = DataParser.GetCpuInfo();
		}
		
		if (_config.Memory)
		{
			log.MemoryLog = DataParser.GetMemoryInfo();
		}

		if (_config.Storage)
		{
			log.StorageLogs = DataParser.GetStorageInfo().ToList();
		}

		if (_config.Network)
		{
			Console.WriteLine("");
		}
		
		if (_config.Services != null)
		{
			var serviceLogs = _config.Services.Select(s => DataParser.GetServiceInfo(s, _lastLogDate)).ToList();
			log.ServiceLogs = serviceLogs;
			File.WriteAllText(LastLogDateFilename, DateTime.Now.ToString("u"));
			_lastLogDate = DateTime.Now;
		}
		
		return log;
	}
}