namespace YALM.Common.Models.Graphql.Logs;

public class CpuLog : LogBase
{
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public CpuLog(DateTime date, double usage, int numberOfTasks) : base(date)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
		Date = date;
	}
}