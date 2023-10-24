namespace API.Models.Db;

public abstract class LogBase
{ 
	public int ServerId { get; }
	public DateTime Date { get; }
	public int Interval { get; }
	
	protected LogBase(int serverId, DateTime date, int interval)
	{
		ServerId = serverId;
		Date = date;
		Interval = interval;
	}
}