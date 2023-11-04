using Common.Models.Graphql.OutputModels;

namespace Common.Models.Graphql.InputModels;

public class MemoryInput : MemoryOutput, IDbLogBase
{
	public int Interval { get; }
	
	public MemoryInput(int serverId, DateTime date, int interval, int mbUsed, int mbTotal) : base(serverId, date, mbUsed, mbTotal)
	{
		Interval = interval;
	}
}