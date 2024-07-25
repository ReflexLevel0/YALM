namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionLogIdInput(int serverId, string uuid, DateTimeOffset date)
{
	public int ServerId { get; } = serverId;
	public string Uuid { get; } = uuid;
	public DateTimeOffset Date { get; } = date;
}