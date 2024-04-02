namespace YALM.Common.Models.Graphql.Logs;

public class MemoryLog : LogBase
{
	public long? TotalKb { get; set; }
	public long? FreeKb { get; set; }
	public long? UsedKb { get; set; }
	public long? SwapTotalKb { get; set; }
	public long? SwapFreeKb { get; set; }
	public long? SwapUsedKb { get; set; }
	public long? AvailableKb { get; set; }
	public long? CachedKb { get; set; }
}