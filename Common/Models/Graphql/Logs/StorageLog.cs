namespace Common.Models.Graphql.Logs;

public class StorageLog : LogBase
{
	public long? Bytes { get; }
	public double? UsedPercentage { get; }

	public StorageLog(DateTime date, long? bytes, double? usedPercentage) : base(date)
	{
		Bytes = bytes;
		UsedPercentage = usedPercentage;
	}
}