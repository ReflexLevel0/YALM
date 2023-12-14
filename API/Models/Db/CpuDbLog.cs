using Common.Models;

namespace API.Models.Db;

public class CpuDbLog : DbLogBase
{
	public int ServerId { get; }
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public CpuDbLog(int serverId, DateTime date, int interval, double usage, int numberOfTasks) : base(date, interval)
	{
		ServerId = serverId;
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}