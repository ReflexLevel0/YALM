using YALM.Monitor.Models.LogInfo;

namespace YALM.Monitor;

public class LogHelper
{
	private readonly DataHelper _dataHelper = new();
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
	
	public async Task<LogBase> Log()
	{
		var log = new LogBase(DateTime.Now);

		if (_config.Cpu)
		{
			log.CpuInfo = await _dataHelper.GetCpuInfo();
		}

		if (_config.Process)
		{
			log.ProcessInfo = await _dataHelper.GetProcessInfo();
		}

		if (_config.Storage)
		{
			log.StorageLogs = _dataHelper.GetStorageInfo().ToList();
		}

		if (_config.Network)
		{
			Console.WriteLine("");
		}
		
		if (_config.Services != null)
		{
			var serviceLogs = _config.Services.Select(s => _dataHelper.GetServiceInfo(s, _lastLogDate)).ToList();
			log.ServiceLogs = serviceLogs;
			await File.WriteAllTextAsync(LastLogDateFilename, DateTime.Now.ToString("u"));
			_lastLogDate = DateTime.Now;
		}
		
		return log;
	}
}