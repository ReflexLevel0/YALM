namespace Common.Models;

public class LogBase
{
	public DateTime Date { get; set; }

	public LogBase(DateTime date)
	{
		Date = date;
	}
}