namespace API.Models;

public class Cpu : Log
{
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public Cpu(int serverId, DateTime date, int interval, double usage, int numberOfTasks) : base(serverId, date, interval)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}