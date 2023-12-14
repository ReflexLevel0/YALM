using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.InputModels;

public class PartitionLogInput : PartitionLog, IDbLogBase
{
	public int DiskId { get; }
	public int Interval { get; }
	public string Uuid { get; }

	public PartitionLogInput(int diskId, DateTime date, int interval, string uuid, long? bytes, double? usedPercentage) : base(date, bytes, usedPercentage)
	{
		DiskId = diskId;
		Interval = interval;
		Uuid = uuid;
	}
}