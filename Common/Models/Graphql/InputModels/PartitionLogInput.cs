using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionLogInput(int serverId, int interval, string partitionUuid) : PartitionLog, ILog
{
	public int ServerId { get; } = serverId;
	public int Interval { get; } = interval;
	public string PartitionUuid { get; } = partitionUuid;
}