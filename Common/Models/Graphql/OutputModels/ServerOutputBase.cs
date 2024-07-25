namespace YALM.Common.Models.Graphql.OutputModels;

public class ServerOutputBase(int serverId)
{
	public int ServerId { get; set; } = serverId;
}