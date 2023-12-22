namespace YALM.Common.Models.Graphql.Logs;

public class PartitionLog : LogBase
{
	public long? Bytes { get; set; }
	public double? UsedPercentage { get; set; }
}