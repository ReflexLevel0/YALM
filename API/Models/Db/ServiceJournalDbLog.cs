using Common.Models;

namespace API.Models.Db;

public class ServiceJournalDbLog : DbLogBase
{
	public int ServerId { get; }
	public string ServiceName { get; }
	public string LogText { get; }
	
	public ServiceJournalDbLog(int serverId, DateTime date, int interval, string serviceName, string logText) : base(date, interval)
	{
		ServerId = serverId;
		ServiceName = serviceName;
		LogText = logText;
	}
}