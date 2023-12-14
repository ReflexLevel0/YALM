namespace Common.Models.Graphql.Logs;

public class MemoryLog : LogBase
{
	public int MbUsed { get; }
	public int MbTotal { get; }

	public MemoryLog(DateTime date, int mbUsed, int mbTotal) : base(date)
	{
		MbUsed = mbUsed;
		MbTotal = mbTotal;
	}
}