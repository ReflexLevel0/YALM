namespace YALM.Common.Models;

public abstract class Log : ILog
{ 
	public DateTime Date { get; }
	public int Interval { get; }
	
	protected Log(DateTime date, int interval)
	{
		Date = date;
		Interval = interval;
	}
}