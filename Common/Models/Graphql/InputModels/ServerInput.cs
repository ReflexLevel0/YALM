namespace YALM.Common.Models.Graphql.InputModels;

public class ServerInput(int serverId)
{
	public int ServerId { get; set; } = serverId;
}