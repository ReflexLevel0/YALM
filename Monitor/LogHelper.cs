using YALM.Monitor.Exceptions;
using YALM.Monitor.Models.LogInfo;
using DateTime = System.DateTime;

namespace YALM.Monitor;

public class LogHelper
{
	private readonly DataHelper _dataHelper = new();
	private readonly Config _config;
	private readonly string _lastLogDateFilename;
	private DateTime? _lastLogDate;
	
	public LogHelper(Config config, string lastLogDateFilename)
	{
		_config = config;
		_lastLogDateFilename = lastLogDateFilename;
		if (File.Exists(_lastLogDateFilename))
		{
			_lastLogDate = DateTime.Parse(File.ReadAllText(_lastLogDateFilename));
		}
	}
	
	public async Task<LogBase> Log()
	{
		int sleepMillis = _config.IntervalInMinutes * 5000;
		DateTime? nextLogDate = _lastLogDate?.AddMinutes(_config.IntervalInMinutes) ?? DateTime.Now;
		nextLogDate = nextLogDate.Value.AddSeconds(-nextLogDate.Value.Second);
		nextLogDate = nextLogDate.Value.AddMilliseconds(-nextLogDate.Value.Millisecond);
		nextLogDate = nextLogDate.Value.AddMicroseconds(-nextLogDate.Value.Microsecond);
		
		//If a log should already have been taken more then the interval ago 
		if (DateTime.Now > nextLogDate && DateTime.Now.Subtract((DateTime)nextLogDate).TotalMinutes >= _config.IntervalInMinutes)
		{
			nextLogDate = DateTime.Now;
		}
			
		Console.WriteLine($"Next log time: {nextLogDate}");
			
		//Sleeping for a section of the interval if logging interval hasn't passed yet
		while (DateTime.Now <= (DateTime)nextLogDate)
		{
			Thread.Sleep(sleepMillis);
		}
		
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
		}
		
		//Setting last log date to the current time
		await File.WriteAllTextAsync(_lastLogDateFilename, DateTime.UtcNow.ToString("u"));
		_lastLogDate = nextLogDate;
		
		return log;
	}
}