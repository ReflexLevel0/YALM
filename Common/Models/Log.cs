namespace YALM.Common.Models;

public abstract class Log : ILog
{ 
	public DateTimeOffset Date { get; }
	public int Interval { get; }
	
	protected Log(DateTimeOffset date, int interval)
	{
		Date = date;
		Interval = interval;
	}
}