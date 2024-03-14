namespace YALM.Common.Models.Graphql.InputModels;

public class CpuLogIdInput(int serverId, DateTime date)
{
	public int ServerId { get; } = serverId;
	public DateTime Date { get; } = date;
}