namespace YALM.Common.Models.Graphql.OutputModels;

public class ServerOutput(int serverId, bool online) : ServerOutputBase(serverId)
{
	public bool Online { get; set; } = online;
}