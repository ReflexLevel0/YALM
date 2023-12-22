namespace YALM.Common.Models;

public class LogBase
{
	public DateTime Date { get; set; }

	public LogBase()
	{
		
	}
	
	public LogBase(DateTime date)
	{
		Date = date;
	}
}