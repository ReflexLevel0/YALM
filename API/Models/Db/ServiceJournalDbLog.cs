using Common.Models;

namespace API.Models.Db;

public class ServiceJournalDbLog : DbLogBase
{
	public string ServiceName { get; }
	public string LogText { get; }
	
	public ServiceJournalDbLog(int serverId, DateTime date, int interval, string serviceName, string logText) : base(serverId, date, interval)
	{
		ServiceName = serviceName;
		LogText = logText;
	}
}