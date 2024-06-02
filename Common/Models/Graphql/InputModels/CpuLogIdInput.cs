namespace YALM.Common.Models.Graphql.InputModels;

public class CpuLogIdInput(int serverId, DateTimeOffset date)
{
	public int ServerId { get; } = serverId;
	public DateTimeOffset Date { get; } = date;
}