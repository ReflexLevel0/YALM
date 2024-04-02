namespace YALM.Common.Models.Graphql.InputModels;

public class MemoryLogIdInput(int serverId)
{
	public int ServerId { get; } = serverId;
}