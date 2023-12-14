using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.InputModels;

public class MemoryInput : MemoryLog, IDbLogBase
{
	public int ServerId { get; }
	public int Interval { get; }

	public MemoryInput(int serverId, int interval, DateTime date, int mbUsed, int mbTotal) : base(date, mbUsed, mbTotal)
	{
		ServerId = serverId;
		Interval = interval;
	}
}