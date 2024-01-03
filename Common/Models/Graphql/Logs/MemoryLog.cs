namespace YALM.Common.Models.Graphql.Logs;

public class MemoryLog : LogBase
{
	public int? UsedKb { get; set; }
	public decimal? UsedPercentage { get; set; }
	public int? SwapUsedKb { get; set; }
	public int? CachedKb { get; set; }
}