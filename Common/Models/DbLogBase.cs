namespace Common.Models;

public abstract class DbLogBase : IDbLogBase
{ 
	public DateTime Date { get; }
	public int Interval { get; }
	
	protected DbLogBase(DateTime date, int interval)
	{
		Date = date;
		Interval = interval;
	}
}