namespace Common.Models.Graphql.Logs;

public class PartitionLog : LogBase
{
	public long? Bytes { get; }
	public double? UsedPercentage { get; }

	public PartitionLog(DateTime date, long? bytes, double? usedPercentage) : base(date)
	{
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}
}