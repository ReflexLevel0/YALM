namespace API.Models;

public class CpuLog : LogBase
{
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public CpuLog(int serverId, DateTime date, int interval, double usage, int numberOfTasks) : base(serverId, date, interval)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}