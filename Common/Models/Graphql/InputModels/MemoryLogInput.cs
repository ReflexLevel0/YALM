using YALM.Common.Models.Graphql.Logs;

namespace YALM.Common.Models.Graphql.InputModels;

public class MemoryLogInput : MemoryLog, IDbLogBase
{
	public int ServerId { get; }
	public int Interval { get; }

	public MemoryLogInput(int serverId, int interval, DateTime date, int mbUsed, int mbTotal) : base(date, mbUsed, mbTotal)
	{
		ServerId = serverId;
		Interval = interval;
	}
}