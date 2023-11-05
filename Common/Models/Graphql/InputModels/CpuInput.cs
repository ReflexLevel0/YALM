using Common.Models.Graphql.OutputModels;

namespace Common.Models.Graphql.InputModels;

public class CpuInput : CpuOutput, IDbLogBase
{
	public int Interval { get; }

	public CpuInput(int serverId, DateTime date, int interval, double usage, int numberOfTasks) : base(serverId, date, usage, numberOfTasks)
	{
		Interval = interval;
	}
}