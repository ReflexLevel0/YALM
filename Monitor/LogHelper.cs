using YALM.Monitor.Exceptions;
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

		if (_config.Program)
		{
			log.ProgramInfo = await _dataHelper.GetProgramInfo();
		}

		if (_config.Storage)
		{
			log.StorageLogs = new List<StorageLog>();
			await foreach (var l in _dataHelper.GetStorageInfo())
			{
				log.StorageLogs.Add(l);
			}
		}

		if (_config.Network)
		{
			Console.WriteLine("");
		}
		
		if (_config.Services != null)
		{
			var serviceLogs = new List<ServiceLog>();
			foreach (var service in _config.Services)
			{
				try
				{
					serviceLogs.Add(await _dataHelper.GetServiceInfo(service, _lastLogDate));
				}
				catch(ServiceNotFoundException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}	
			log.ServiceLogs = serviceLogs;
			await File.WriteAllTextAsync(LastLogDateFilename, DateTime.Now.ToString("u"));
			_lastLogDate = DateTime.Now;
		}
		
		return log;
	}
}