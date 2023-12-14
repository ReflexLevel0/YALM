namespace Common.Models.Graphql.OutputModels;

public class CpuOutputBase
{
	public int ServerId { get; set; }

	public CpuOutputBase(int serverId)
	{
		ServerId = serverId;
	}
}