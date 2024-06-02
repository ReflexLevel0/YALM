namespace YALM.Common.Models;

public class LogBase
{
	public DateTimeOffset Date { get; set; }

	public LogBase()
	{
		
	}
	
	public LogBase(DateTimeOffset date)
	{
		Date = date;
	}
}