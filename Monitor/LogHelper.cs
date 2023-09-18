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
			//TODO: log storage
			Console.WriteLine("df -H");
		}

		if (_config.Network)
		{
			//TODO: figure out a way to track network usage (this might require sudo privileges)
			Console.WriteLine("");
		}
		
		if (_config.Services != null)
		{
			foreach (string serviceName in _config.Services)
			{
				log.ServiceLogs.Add(DataParser.GetServiceInfo(serviceName));
			}
		}
		
		return log;
	}
}