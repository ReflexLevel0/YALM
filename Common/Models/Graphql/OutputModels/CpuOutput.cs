namespace Common.Models.Graphql.OutputModels;

public class CpuOutput : GraphqlModelBase
{
	public double Usage { get; }
	public int NumberOfTasks { get; }
	
	public CpuOutput(int serverId, DateTime date, double usage, int numberOfTasks) : base(serverId, date)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}