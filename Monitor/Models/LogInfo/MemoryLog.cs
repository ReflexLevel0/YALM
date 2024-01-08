namespace YALM.Monitor.Models.LogInfo;

public class MemoryLog
{
	public long? MemoryTotalKb { get; set; }
	public long? SwapTotalKb { get; set; }
	public long? SwapFreeKb { get; set; }
	public long? SwapUsedKb { get; set; }
	public long? AvailableMemoryKb { get; set; }
	public long? MemoryFreeKb { get; set; }
	public long? MemoryUsedKb { get; set; }
	public long? CachedKb { get; set; }
}