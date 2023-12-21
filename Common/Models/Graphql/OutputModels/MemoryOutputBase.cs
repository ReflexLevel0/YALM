namespace YALM.Common.Models.Graphql.OutputModels;

public class MemoryOutputBase
{
	public int ServerId { get; set; }
	
	public MemoryOutputBase(int serverId)
	{
		ServerId = serverId;
	}
}