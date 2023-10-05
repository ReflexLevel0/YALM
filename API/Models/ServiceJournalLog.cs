namespace API.Models;

public class ServiceJournalLog : LogBase
{
	public string ServiceName { get; }
	public string LogText { get; }
	
	public ServiceJournalLog(int serverId, DateTime date, int interval, string serviceName, string logText) : base(serverId, date, interval)
	{
		ServiceName = serviceName;
		LogText = logText;
	}
}