using Common.Models.Graphql.Logs;

namespace Common.Models.Graphql.InputModels;

public class CpuInput : CpuLog, IDbLogBase
{
	public int ServerId { get; }
	public int Interval { get; }
	
	public CpuInput(int serverId, int interval, DateTime date, double usage, int numberOfTasks) : base(date, usage, numberOfTasks)
	{
		ServerId = serverId;
		Interval = interval;
	}
}