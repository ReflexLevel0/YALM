using Common.Models;

namespace API.Models.Db;

public class PartitionDbLog : DbLogBase
{
	public int DiskId { get; }
	public string UUID { get; }
	public long Bytes { get; }
	public double UsedPercentage { get; }

	public PartitionDbLog(int diskId, DateTime date, int interval, string uuid, long bytes, double usedPercentage) : base(date, interval)
	{
		DiskId = diskId;
		UUID = uuid;
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}
}