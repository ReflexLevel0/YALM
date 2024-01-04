using System.Text;

namespace YALM.Monitor.Models.LogInfo;

public class MemoryInfo
{
	public ulong? MemoryTotalKb { get; set; }
	public ulong? MemoryFreeKb { get; set; }
	public ulong? MemoryUsedKb { get; set; }
	public ulong? CachedKb { get; set; }
	public ulong? SwapTotalKb { get; set; }
	public ulong? SwapFreeKb { get; set; }
	public ulong? SwapUsedKb { get; set; }
	public ulong? AvailableMemoryKb { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder(512);
		builder.AppendLine($"Memory: {MemoryTotalKb}KB total, {MemoryFreeKb}KB free, {MemoryUsedKb}KB used, {CachedKb}KB cached, {AvailableMemoryKb}KB available");
		builder.AppendLine($"Swap: {SwapTotalKb}KB total, {SwapFreeKb}KB free, {SwapUsedKb}KB used");
		return builder.ToString();
	}
}