using Common.Models;

namespace API.Models.Db;

public class PartitionDbLog : DbLogBase
{
	public string UUID { get; }
	public long Bytes { get; }
	public double UsedPercentage { get; }

	public PartitionDbLog(int serverId, DateTime date, int interval, string uuid, long bytes, double usedPercentage) : base(serverId, date, interval)
	{
		UUID = uuid;
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}
}