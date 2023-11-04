namespace Common.Models.Graphql.OutputModels;

public class MemoryOutput : GraphqlModelBase
{
	public int MbUsed { get; }
	public int MbTotal { get; }
	
	public MemoryOutput(int serverId, DateTime date, int mbUsed, int mbTotal) : base(serverId, date)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}