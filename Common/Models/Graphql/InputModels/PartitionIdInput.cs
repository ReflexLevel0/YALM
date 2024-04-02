namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionIdInput(int serverId, string uuid)
{
	public int ServerId { get; } = serverId;
	public string Uuid { get; } = uuid;
}