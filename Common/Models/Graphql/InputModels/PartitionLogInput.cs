using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.InputModels;

public class PartitionLogInput : PartitionLog, IDbLogBase
{
	public int ServerId { get; }
	public int Interval { get; }
	public string Uuid { get; }

	public PartitionLogInput(int serverId, DateTime date, int interval, string uuid, long? bytes, double? usedPercentage) : base(date, bytes, usedPercentage)
	{
		ServerId = serverId;
		Interval = interval;
		Uuid = uuid;
	}
}