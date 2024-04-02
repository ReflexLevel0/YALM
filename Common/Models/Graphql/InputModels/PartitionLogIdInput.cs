namespace YALM.Common.Models.Graphql.InputModels;

public class PartitionLogIdInput(int serverId, string uuid, DateTime date)
{
	public int ServerId { get; } = serverId;
	public string Uuid { get; } = uuid;
	public DateTime Date { get; } = date;
}