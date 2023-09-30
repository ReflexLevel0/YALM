namespace API.Models;

public class ServiceLog : Log
{
	public string ServiceName { get; }
	public string LogText { get; }
	
	public ServiceLog(int serverId, DateTime date, int interval, string serviceName, string logText) : base(serverId, date, interval)
	{
		ServiceName = serviceName;
		LogText = logText;
	}
}