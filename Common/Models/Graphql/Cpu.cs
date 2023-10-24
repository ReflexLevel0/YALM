namespace Common.Models.Graphql;

public class Cpu : GraphqlModelBase
{
	public double Usage { get; }
	public int NumberOfTasks { get; }

	public Cpu(int serverId, DateTime date, double usage, int numberOfTasks) : base(serverId, date)
	{
		Usage = usage;
		NumberOfTasks = numberOfTasks;
	}
}