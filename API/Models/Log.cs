namespace API.Models;

public abstract class Log
{ 
	public int ServerId { get; }
	public DateTime Date { get; }
	public int Interval { get; }
	
	protected Log(int serverId, DateTime date, int interval)
	{
		ServerId = serverId;
		Date = date;
		Interval = interval;
	}
}