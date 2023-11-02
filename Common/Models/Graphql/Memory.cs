namespace Common.Models.Graphql;

public class Memory : GraphqlModelBase
{
	public int MbUsed { get; }
	public int MbTotal { get; }

	protected Memory(int serverId, DateTime date, int mbUsed, int mbTotal) : base(serverId, date)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}