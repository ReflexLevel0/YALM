namespace Monitor;

public class LogHelper
{
	private readonly Config _config;

	public LogHelper(Config config)
	{
		_config = config;
	}
	
	public Log Log()
	{
		var log = new Log(DateTime.Now);

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
			var serviceLogs = _config.Services.Select(DataParser.GetServiceInfo).ToList();
			log.ServiceLogs = serviceLogs;
		}
		
		return log;
	}
}