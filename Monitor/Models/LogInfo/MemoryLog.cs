namespace YALM.Monitor.Models.LogInfo;

public class MemoryLog
{
	public ulong? MemoryTotalKb { get; set; }
	public ulong? SwapTotalKb { get; set; }
	public ulong? SwapFreeKb { get; set; }
	public ulong? SwapUsedKb { get; set; }
	public ulong? AvailableMemoryKb { get; set; }
	public ulong? MemoryFreeKb { get; set; }
	public ulong? MemoryUsedKb { get; set; }
	public ulong? CachedKb { get; set; }
}