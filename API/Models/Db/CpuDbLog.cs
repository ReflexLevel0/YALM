using Common.Models;

namespace API.Models.Db;

public class CpuDbLog : DbLogBase
{
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public CpuDbLog(int serverId, DateTime date, int interval, double usage, int numberOfTasks) : base(serverId, date, interval)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}