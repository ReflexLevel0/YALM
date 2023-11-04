namespace Common.Models;

public abstract class DbLogBase : IDbLogBase
{ 
	public int ServerId { get; }
	public DateTime Date { get; }
	public int Interval { get; }
	
	protected DbLogBase(int serverId, DateTime date, int interval)
	{
		ServerId = serverId;
		Date = date;
		Interval = interval;
	}
}